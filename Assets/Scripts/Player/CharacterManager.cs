using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    //public Player Player; // PlayerMovement 말고 Player.cs가 있어야 함

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
