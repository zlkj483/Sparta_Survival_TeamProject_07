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
        //Inventory inventory = CharacterManager.Instance.inventory;
        //if (inventory != null)
        //{
        //    inventory.AddItem(data);
        //}
        //else
        //{
        //  Debug.LogWarning("Inventory가 없습니다!");
        //}
        Destroy(gameObject);
    }
}
