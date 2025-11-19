using UnityEngine;

[CreateAssetMenu(menuName = "Game/Monster Data", fileName = "MonsterData_")]
public class MonsterData : ScriptableObject
{
    [Header("ID / 이름")]
    public string monsterId;      // 내부 식별용
    public string displayName;    // UI, 디버그용 이름

    [Header("비주얼 / 프리팹")]
    public GameObject skinPrefab;                         // FBX 기반 프리팹

    [Header("기본 스탯")]
    public float maxHp = 100f;
    public float moveSpeed = 3.5f;
    public float baseDamage = 10f;

    [Header("AI 범위 설정")]
    public float aggroRange = 10f;   // 플레이어 감지 범위
    public float attackRange = 2f;   // 공격 거리
    public float giveUpRange = 20f;  // 추격 포기 거리
    public float sightRange = 12f;
    public float sightAngle = 120f;
    public LayerMask targetLayer;
    public LayerMask obstacleMask;

    [Header("패트롤 설정")]
    public float patrolWaitTime = 2f;

    [Header("공격 패턴")]
    public float attackCooldown = 2f;

    [Header("드랍/죽음")]
    public DropTable dropTable;
    public float destroyAfterDeath = 3f;

    [Header("보스 전용")]
    public bool isBoss;
}