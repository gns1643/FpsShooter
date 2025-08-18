using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool; // 오브젝트 풀링을 위한 라이브러리

public class EnemySpawner : MonoBehaviour
{
    public enum ZombieType
    {
        Normal,
        Fast,
        Tank
    }

    [Header("플레이어 트랜스폼 지정")]
    [SerializeField] private Transform playerTransform;
    [Header("플레이어 스테이터스 지정")]
    [SerializeField] private PlayerStatus playerStatus;
    [Header("적 프리팹 지정")]
    [SerializeField] private GameObject[] zombiePrefab; // 스폰할 좀비 프리팹 (0 : 일반, 1 : 빠른, 2 : 탱커)
    [Header("땅 레이어 지정")]
    [SerializeField] private LayerMask groundMask;

    [Header("스폰 정보 지정")]
    [SerializeField] private float spawnInterval ; // 좀비 스폰 간격(초)
    [SerializeField] private float spawnDistance ; // 스포너로부터 좀비 스폰 거리
    [Header("풀 정보 지정")]
    [SerializeField] private int initPoolSize;
    [SerializeField] private int maxPoolSize;
    private ObjectPool<GameObject>[] zombiePools = new ObjectPool<GameObject>[3];


    void Awake()
    {
        for (int i = 0; i < zombiePools.Length; i++)
        {//시작할 때 각 좀비 종류 오브젝트 풀 초기화
            int index = i; // 람다 캡쳐 방지용
            zombiePools[i] = new ObjectPool<GameObject>(
                () => CreateZombie(index),
                obj => OnGetZombie(obj, index),
                obj => OnReleaseZombie(obj, index),
                obj => OnDestroyZombie(obj, index),
                true, initPoolSize, maxPoolSize);
        }
    }

    public void StartAutoSpawn(ZombieType type, int count)
    {
        StartCoroutine(AutoSpawnCoroutine(type, count));
    }

    IEnumerator AutoSpawnCoroutine(ZombieType type, int count)
    {
        int spawned = 0;
        while (spawned < count)
        {
            SpawnEnemy(type);
            spawned++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnEnemy(ZombieType type)
    {
        zombiePools[(int)type].Get();
        WaveManager.Instance.OnZombieSpawned();
        WaveManager.Instance.zombiesSpawnedCount++; 
        WaveManager.Instance.PrintZombieCount(); //디버그용
    }

    GameObject CreateZombie(int index)
    {
        Debug.Log("실행");
        var obj = Instantiate(zombiePrefab[index]);
        obj.SetActive(false);

        obj.GetComponent<Zombie>().SetPool(zombiePools[index]);
        //생성된 프리펩에 플레이어 위치 지정
        obj.GetComponent<Zombie>().SetPlayerTransform(playerTransform, playerStatus);

        return obj;
    }

    void OnGetZombie(GameObject obj, int index)
    {
        obj.SetActive(true);

        // NavMeshAgent 컴포넌트 가져오기
        NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();

        // NavMeshAgent 비활성화 (이동 막기 위해)
        if (agent != null)
            agent.enabled = false;

        // 위치 세팅 (예: NavMesh.SamplePosition 이용)
        Vector3 randomSpawnPos = transform.position + Random.insideUnitSphere * spawnDistance;
        randomSpawnPos.y += 5f;

        RaycastHit groundHit;
        Vector3 groundPos = randomSpawnPos;
        if (Physics.Raycast(randomSpawnPos, Vector3.down, out groundHit, 10f, groundMask))
            groundPos = groundHit.point;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(groundPos, out navHit, 2f, NavMesh.AllAreas))
            obj.transform.position = navHit.position;
        else
            obj.transform.position = groundPos;

        var zombie = obj.GetComponent<Zombie>();
        zombie.currentHp = zombie.maxHp;

        // Delay 후 NavMeshAgent 활성화 시작 (예: Coroutine)
        obj.GetComponent<MonoBehaviour>().StartCoroutine(EnableAgentDelayed(agent, 0.1f));
    }

    private IEnumerator EnableAgentDelayed(NavMeshAgent agent, float delay)
    {
        if (agent == null)
            yield break;

        yield return new WaitForSeconds(delay);
        agent.enabled = true;
    }

    void OnReleaseZombie(GameObject obj, int index) => obj.SetActive(false);

    void OnDestroyZombie(GameObject obj, int index) => Destroy(obj);

}
