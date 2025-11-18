using UnityEngine;

public class PlayerGather : MonoBehaviour
{
    [SerializeField] private Camera cam; // 플레이어 카메라
    [SerializeField] private float gatherRange = 3f; // 때릴 수 있는 거리
    [SerializeField] private int damage = 5; // 데미지 값 (원하면 도끼마다 다르게 변경)

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 좌클릭
        {
            TryGather();
        }
    }

    void TryGather()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, gatherRange))
        {
            Structure structure = hit.collider.GetComponent<Structure>();

            if (structure != null)
            {
                // 여기에서 3개를 넘김
                structure.Gather(
                    hit.point,      // 때린 위치
                    hit.normal,     // 표면 방향
                    damage          // 데미지
                );
            }
        }
    }
}
