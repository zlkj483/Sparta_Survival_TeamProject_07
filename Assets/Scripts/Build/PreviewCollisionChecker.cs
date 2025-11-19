using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCollisionChecker : MonoBehaviour
{
    [HideInInspector]
    public bool isColliding = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ground"))
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ground"))
        {
            isColliding = false;
        }
    }
}
