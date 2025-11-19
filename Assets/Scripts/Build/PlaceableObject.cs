using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public void Rotate90()
    {
        transform.Rotate(0f, 90f, 0f, Space.World);
    }
}
