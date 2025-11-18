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
        //CharacterManager.Instance.Player.itemData = data;
        //CharacterManager.Instance.Player.addItem?.Invoke();

        Destroy(gameObject);
    }
}
