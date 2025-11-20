using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    void Interact();
}
public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetInteractPrompt()
    {
        string info = $"{data.displayName}\n {data.description}";
        return info;
    }

    public void Interact()
    {
        if (UIInventory.Instance != null)
        {
            UIInventory.Instance.AddItem(data);
        }
        else
        {
            Debug.LogWarning("UIInventory.Instance가 없습니다!");
        }

        Destroy(gameObject);
    }
}
