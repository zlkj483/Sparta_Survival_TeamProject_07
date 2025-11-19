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
    public static ItemObject instance;
    public ItemData data;

    public string GetInteractPrompt()
    {
        string info = $"{data.displayName}\n {data.description}";
        return info;
    }

    public void Interact()
    {
        UIInventory inventory = CharacterManager.Instance.inventory;
        if (inventory != null)
        {
            inventory.AddItem();
        }
        else
        {
            Debug.LogWarning("Inventory가 없습니다!");
        }
        Destroy(gameObject);
    }
}
