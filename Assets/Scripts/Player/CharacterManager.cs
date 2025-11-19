using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;
    // public Inventory inventory;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
