using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneLoader : MonoBehaviour
{

    [Header("넘어갈 씬 이름")]
    
    public string mainSceneName = "MainScene";

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            LoadMainScene();
        }
    }

    public void LoadMainScene()
    {
        if (string.IsNullOrEmpty(mainSceneName))
        {
            Debug.LogError("메인 씬 이름이 설정되지 않았습니다.");
            return;
        }

        // 씬 로드
        SceneManager.LoadScene(mainSceneName);
        Debug.Log($"씬 전환: {mainSceneName} ");
    }
}

