using UnityEngine;

public class PlayerGather : MonoBehaviour
{
    [SerializeField] private Camera cam; // 플레이어 카메라
    [SerializeField] private float gatherRange = 3f; // 때릴 수 있는 거리
    [SerializeField] private int damage = 5; // 데미지 값 

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
            IGatherable gatherTarget = hit.collider.GetComponent<IGatherable>();

            if (gatherTarget != null)
            {
                gatherTarget.Gather(hit.point, hit.normal, damage);
            }
        }
    }
}
