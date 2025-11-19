using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCollisionChecker : MonoBehaviour
{
    [HideInInspector]
    public bool isColliding = false;

    public LayerMask groundLayer;

    private void Awake()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != groundLayer)
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != groundLayer)
        {
            isColliding = false;
        }
    }
}