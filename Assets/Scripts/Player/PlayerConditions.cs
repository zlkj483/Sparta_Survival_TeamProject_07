using System;
using UnityEngine;
using TMPro;

public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UICondition uiCondition;
    public TextMeshProUGUI tempText;
    public float environmentTemperature;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirst { get { return uiCondition.thirst; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float noHungerHealthDecay;
    public event Action onTakeDamage;

    private void Update()
    {
        float coldMul = GetColdMultiplier();
        hunger.Subtract(hunger.passiveValue * coldMul * Time.deltaTime);
        thirst.Subtract(thirst.passiveValue * coldMul * Time.deltaTime);

        float staminaRegen = stamina.passiveValue * (2f - coldMul);
        stamina.Add(staminaRegen * Time.deltaTime);

        if (hunger.curValue <= 0f || thirst.curValue <= 0f)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if (health.curValue <= 0f)
        {
            Die();
        }
        if (tempText != null)
            tempText.text = environmentTemperature.ToString("F1") + "C";
    }
    public void TakePhysicalDamage(float damage)
    {
        health.Subtract(damage);
        onTakeDamage?.Invoke();
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }
    public void Drink(float amount)
    {
        thirst.Add(amount);
    }

    public void Die()
    {
        Debug.Log("?”Œ? ˆ?´?–´ê°? ì£½ì—ˆ?‹¤.");
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }
        stamina.Subtract(amount);
        return true;
    }
    // ¿Âµµ ÆÐ³ÎÆ¼ °è»ê
    float GetColdMultiplier()
    {
        if (environmentTemperature >= 5f) return 1f;
        float t = Mathf.InverseLerp(-20f, 5f, environmentTemperature);
        return Mathf.Lerp(1.5f, 1f, t); // Ãß¿ï¼ö·Ï 1.5¹è
    }
}