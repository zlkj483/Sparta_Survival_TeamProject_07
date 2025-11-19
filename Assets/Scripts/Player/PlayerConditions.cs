using System;
using UnityEngine;
using TMPro;

public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UICondition uiCondition;
    public TextMeshProUGUI tempText; // UI TextMeshPro
    public float environmentTemperature; // DayNightCycle에서 받음
    public float noHungerHealthDecay = 1f;
    public event Action onTakeDamage;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirst { get { return uiCondition.thirst; } }
    Condition stamina { get { return uiCondition.stamina; } }

    private void Update()
    {
        // 환경 온도 패널티
        float coldMul = GetColdMultiplier();

        // 배고픔/갈증은 추울수록 더 빨리 감소
        hunger.Subtract(hunger.passiveValue * coldMul * Time.deltaTime);
        thirst.Subtract(thirst.passiveValue * coldMul * Time.deltaTime);

        // 스태미나 재생은 추울수록 느려짐
        float staminaRegen = stamina.passiveValue * (2f - coldMul);
        stamina.Add(staminaRegen * Time.deltaTime);

        // 배고픔/갈증 0이면 체력 감소
        if (hunger.curValue <= 0f || thirst.curValue <= 0f)
            health.Subtract(noHungerHealthDecay * Time.deltaTime);

        // 체력 0이면 사망
        if (health.curValue <= 0f)
            Die();

        // UI 표시
        if (tempText != null)
            tempText.text = environmentTemperature.ToString("F1") + "C";
    }

    public void Heal(float amount) => health.Add(amount);
    public void Eat(float amount) => hunger.Add(amount);
    public void Drink(float amount) => thirst.Add(amount);

    public void Die() => Debug.Log("플레이어가 죽었다.");
    public void TakePhysicalDamage(float damage)
    {
        health.Subtract(damage);
        onTakeDamage?.Invoke();
    }
    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f) return false;
        stamina.Subtract(amount);
        return true;
    }

    // 온도 패널티 계산
    float GetColdMultiplier()
    {
        if (environmentTemperature >= 5f) return 1f;
        float t = Mathf.InverseLerp(-20f, 5f, environmentTemperature);
        return Mathf.Lerp(3f, 1f, t); // 추울수록 3배
    }
}
