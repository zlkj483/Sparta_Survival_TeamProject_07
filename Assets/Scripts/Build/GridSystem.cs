using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }

    [Header("Player")]
    public Transform player;     // 플레이어 Transform 연결

    [Header("Grid Settings")]
    public int gridRange = 10;   // 플레이어 중심으로 좌우 몇 칸까지 그릴지
    public float cellSize = 1f;

    private void Awake()
    {
        Instance = this;
    }

    public void GetXY(Vector3 worldPos, out int x, out int y)
    {
        Vector3 relativePos = worldPos - player.position;

        x = Mathf.FloorToInt(relativePos.x / cellSize);
        y = Mathf.FloorToInt(relativePos.z / cellSize);
    }

    public Vector3 GetCellCenterWorld(int x, int y)
    {
        Vector3 offset = new Vector3(x * cellSize, 0, y * cellSize);
        return player.position + offset + new Vector3(cellSize / 2f, 0f, cellSize / 2f);
    }

    public bool IsValidGridPosition(int x, int y)
    {
        return Mathf.Abs(x) <= gridRange && Mathf.Abs(y) <= gridRange;
    }
    
    private void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.gray;

        for (int x = -gridRange; x <= gridRange; x++)
        {
            for (int y = -gridRange; y <= gridRange; y++)
            {
                Vector3 pos = GetCellCenterWorld(x, y);
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0.01f, cellSize));
            }
        }
    }
}
