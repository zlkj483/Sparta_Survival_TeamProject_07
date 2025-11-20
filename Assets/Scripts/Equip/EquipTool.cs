using UnityEngine;
using static UnityEngine.UI.Image;

public class EquipTool : Equip
{
    public float attackDistance = 3f;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage = 10;

    private Camera cam;

    private Transform owner;      // 플레이어 Transform

    void Awake()
    {
        cam = Camera.main;
        owner = transform.root;   // 장비가 Player 밑에 붙어있으면 root = Player
    }

    public override void OnAttackInput()
    {
        if (owner == null)
            owner = transform.root;

        Vector3 origin = owner.position + Vector3.up * 1f; // 허리~가슴 높이
        float radius = 0.5f;

        if (Physics.SphereCast(origin, radius, owner.forward, out RaycastHit hit, attackDistance))
        {
            // 자원 채집
            if (doesGatherResources && hit.collider.TryGetComponent(out IGatherable resource))
            {
                resource.Gather(hit.point, hit.normal, damage);
            }

            // 데미지
            if (doesDealDamage && hit.collider.TryGetComponent(out IDamagable damageable))
            {
                damageable.TakePhysicalDamage(damage);
            }
        }
    }
}
