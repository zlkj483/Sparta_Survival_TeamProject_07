using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] float cellWidth = 5;
    [SerializeField] float cellHeight = 5;

    [SerializeField] bool visualiseGrid;
    [SerializeField] int distanceFromPlayer = 5;
    [SerializeField] float gizmoSize = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(!visualiseGrid || !Application.isPlaying)
        {
            return;
        }

        Gizmos.color = Color.white;

        for (float x = -distanceFromPlayer; x <= distanceFromPlayer; x += cellWidth)
        {
            for(float y = -distanceFromPlayer; y <= distanceFromPlayer; y += cellHeight)
            {
                for (float z = -distanceFromPlayer; z <= distanceFromPlayer; z += cellWidth)
                {
                    Vector3 position = GetNearestGridPosition(transform.position) + new Vector3(x, y, z);

                    Gizmos.DrawCube(position, Vector3.one * gizmoSize);
                }
            }
        }
    }

    private Vector3 GetNearestGridPosition(Vector3 position)
    {
        float x = Mathf.Round(position.x / cellWidth) * cellWidth;
        float y = Mathf.Round(position.y / cellHeight) * cellHeight;
        float z = Mathf.Round(position.z / cellWidth) * cellWidth;

        return new Vector3(x, y, z);
    }
}
