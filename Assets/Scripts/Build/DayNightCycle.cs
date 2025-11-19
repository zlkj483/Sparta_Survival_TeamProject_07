using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0f, 1f)] public float time;
    public float fullDayLength = 120f; // 120초 = 1일
    public float startTime = 0.4f; // 아침 시작
    private float timeRate;
    public Vector3 noon;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;

    [Header("Player")]
    public PlayerCondition playerCondition;

    private void Start()
    {
        timeRate = 1f / fullDayLength;
        time = startTime;
    }

    private void Update()
    {
        // 시간 업데이트
        time = (time + timeRate * Time.deltaTime) % 1f;

        // 조명 업데이트
        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);

        // 환경 온도 계산
        float envTemp;
        if (time > 0.7f || time < 0.3f) // 밤
            envTemp = -20f;
        else // 낮
            envTemp = 15f;

        // PlayerCondition에 전달
        if (playerCondition != null)
            playerCondition.environmentTemperature = envTemp;
    }

    private void UpdateLighting(Light lightSource, Gradient colorGradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4f;
        lightSource.color = colorGradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
}
