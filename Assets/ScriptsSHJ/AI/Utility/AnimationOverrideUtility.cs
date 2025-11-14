using System.Collections.Generic;
using UnityEngine;

public static class AnimationOverrideUtility
{
    /// <summary>
    /// baseController의 클립 이름과 같은 이름을 가진 sourceClips를 찾아
    /// AnimatorOverrideController를 만들어 반환.
    /// </summary>
    public static AnimatorOverrideController CreateOverride(
        RuntimeAnimatorController baseController,
        AnimationClip[] sourceClips)
    {
        if (baseController == null)
        {
            Debug.LogError("CreateOverride 실패: baseController 가 null 입니다.");
            return null;
        }

        if (sourceClips == null || sourceClips.Length == 0)
        {
            Debug.LogWarning("CreateOverride 경고: sourceClips 가 비어있습니다.");
            return new AnimatorOverrideController(baseController);
        }

        var overrideController = new AnimatorOverrideController(baseController);

        // Base 컨트롤러에서 override 대상 클립 목록 가져오기
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);

        for (int i = 0; i < overrides.Count; i++)
        {
            AnimationClip originalClip = overrides[i].Key;      // Base에서 쓰던 원본
            string stateName = originalClip.name;               // 상태 이름과 동일하게 쓰는 걸 전제로

            // FBX에서 같은 이름의 클립 찾기
            AnimationClip matched = System.Array.Find(
                sourceClips,
                c => c.name == stateName
            );

            if (matched != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(originalClip, matched);
            }
            else
            {
                // 못 찾으면 그냥 원본 그대로 사용 (경고만 띄우기)
                Debug.LogWarning($"Override 매칭 실패: '{stateName}' 이름의 클립을 sourceClips에서 찾지 못했습니다.");
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(originalClip, originalClip);
            }
        }

        overrideController.ApplyOverrides(overrides);
        return overrideController;
    }
}
