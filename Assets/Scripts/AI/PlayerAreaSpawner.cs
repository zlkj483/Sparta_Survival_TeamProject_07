using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 플레이어 중심으로 동심원 영역을 만들고,
///   - innerRadius 안에는 적이 없음 (안전 구역)
///   - outerRadius 범위 안에만 적을 유지/스폰
/// 플레이어가 움직이면 영역도 같이 움직인다.
/// </summary>
public class PlayerAreaSpawner : MonoBehaviour
{
    [Header("필수 레퍼런스")]
    public Transform player;                 // 플레이어 Transform

    [Header("스폰 반경 설정")]
    public float innerRadius = 15f;          // 안전 구역 반경 (이 안에는 스폰 X)
    public float outerRadius = 40f;          // 스폰/유지 구역 반경
    public float despawnRadiusMultiplier = 1.3f; // 이 거리 밖으로 나간 적은 삭제

    [Header("스폰 관리")]
    public int maxAlive = 15;                // 동시에 존재 가능한 최대 적 수
    public float spawnInterval = 3f;         // 스폰 시도 주기(초)

    [Header("생성 방식")]
    public GameObject enemyPrefab;           // Enemy 프리팹(EnemyAIController 포함)
    public MonsterData[] monsterCandidates;  // 랜덤으로 뽑을 몬스터 데이터들

    [Header("지형 체크")]
    public LayerMask groundMask;             // 땅/지형 레이어 (Raycast로 바닥 찾을 때)
    public float raycastHeight = 50f;        // 플레이어 위에서 이만큼 위에서 쏨

    private readonly List<EnemyAIController> _alive = new();
    private float _timer;

    private void Update()
    {
        if (player == null || enemyPrefab == null)
            return;

        // 1) 현재 살아있는 적 목록 정리 + 너무 멀리 간 적 삭제
        CleanupAndDespawnFar();

        // 2) 최대치면 스폰 안함
        if (_alive.Count >= maxAlive)
            return;

        // 3) 스폰 타이머
        _timer += Time.deltaTime;
        if (_timer < spawnInterval)
            return;

        _timer = 0f;
        TrySpawnOne();
    }

    // 살아있는 리스트 정리 및 플레이어에서 너무 멀리 간 적 삭제
    private void CleanupAndDespawnFar()
    {
        float despawnRadius = outerRadius * despawnRadiusMultiplier;

        for (int i = _alive.Count - 1; i >= 0; i--)
        {
            var enemy = _alive[i];

            if (enemy == null)
            {
                _alive.RemoveAt(i);
                continue;
            }

            float dist = Vector3.Distance(player.position, enemy.transform.position);
            if (dist > despawnRadius)
            {
                Destroy(enemy.gameObject);
                _alive.RemoveAt(i);
            }
        }
    }

    private void TrySpawnOne()
    {
        // 1) 플레이어 주변, innerRadius ~ outerRadius 사이에서 랜덤 위치 뽑기
        Vector3 center = player.position;

        for (int attempt = 0; attempt < 10; attempt++)
        {
            // 랜덤 각도 + 거리
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(innerRadius, outerRadius);

            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            Vector3 rayOrigin = center + offset + Vector3.up * raycastHeight;

            // 2) 위에서 아래로 쏴서 Ground 찾기
            if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundMask))
                continue;

            Vector3 groundPos = hit.point;

            // 3) NavMesh 위로 보정
            if (!NavMesh.SamplePosition(groundPos, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                continue;

            // 4) 실제 스폰
            SpawnEnemy(navHit.position);
            return;
        }

        // 10번 시도했는데도 못 찾으면 이번엔 패스
    }

    private void SpawnEnemy(Vector3 position)
    {
        GameObject go = Instantiate(enemyPrefab, position, Quaternion.identity);

        var controller = go.GetComponent<EnemyAIController>();
        if (controller != null)
        {
            // MonsterData가 프리팹에 이미 들어있으면 생략 가능
            if (controller.monsterData == null)
            {
                controller.monsterData = PickRandomMonsterData();
            }
            _alive.Add(controller);
        }
        else
        {
            Debug.LogWarning("[PlayerAreaSpawner] EnemyPrefab에 EnemyAIController가 없습니다.");
        }
    }

    private MonsterData PickRandomMonsterData()
    {
        if (monsterCandidates == null || monsterCandidates.Length == 0)
            return null;

        int index = Random.Range(0, monsterCandidates.Length);
        return monsterCandidates[index];
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null)
            return;

        Vector3 center = player.position;

        // inner(안전) 구역
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(center, innerRadius);

        // outer(스폰) 구역
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(center, outerRadius);

        // 디스폰 반경
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        Gizmos.DrawWireSphere(center, outerRadius * despawnRadiusMultiplier);
    }
}
