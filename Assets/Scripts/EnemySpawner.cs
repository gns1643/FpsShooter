using System.Collections;
using UnityEngine;
using UnityEngine.Pool; // 오브젝트 풀링을 위한 라이브러리

public class EnemySpawner : MonoBehaviour
{
    [Header("플레이어 트랜스폼 지정")]
    [SerializeField] private Transform playerTransform;
    [Header("적 프리팹 지정")]
    [SerializeField] private GameObject zombiePrefab; // 스폰할 좀비 프리팹
    [Header("스폰 정보 지정")]
    [SerializeField] private float spawnInterval = 2.0f; // 좀비 스폰 간격(초)
    [SerializeField] private float spawnDistance = 3.0f; // 스포너로부터 좀비 스폰 거리
    [Header("풀 정보 지정")]
    [SerializeField] private int initPoolSize;
    [SerializeField] private int maxPoolSize;
    private ObjectPool<GameObject> zombiePool;
    private Coroutine spawnCoroutine;
    

    void Awake()
    {
        //시작할 때 오브젝트 풀 생성
        zombiePool = new ObjectPool<GameObject>(
            CreateZombie, //풀에 더이상 오브젝트가 없을 때 새 오브젝트 생성
            OnGetZombie, //풀에서 꺼낼때 오브젝트 위치 설정
            OnReleaseZombie, //오브젝트를 풀에 반납할 때 실행
            OnDestroyZombie, //오브젝트가 완전히 파괴도리 때 실행
            true,   // 중복 반환 체크
            initPoolSize,     // 최초 생성 개수
            maxPoolSize      // 최대 풀 크기
        );
    }
    void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnWave());
    }
    IEnumerator SpawnWave()
    {
        while (true)
        {
            // 좀비 프리팹 하나 가져오기 
            var zombie = zombiePool.Get();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    GameObject CreateZombie()
    {
        var obj = Instantiate(zombiePrefab);
        obj.SetActive(false);

        obj.GetComponent<Zombie>().SetPool(zombiePool);
        //생성된 프리펩에 플레이어 위치 지정
        obj.GetComponent<Zombie>().SetPlayerTransform(playerTransform);

        return obj;
    }

    void OnGetZombie(GameObject obj)
    {
        obj.SetActive(true);
        obj.transform.position = GetRandomSpawnPosition();
        //체력, 상태 리셋 코드 필요
    }

    void OnReleaseZombie(GameObject obj) => obj.SetActive(false);

    void OnDestroyZombie(GameObject obj) => Destroy(obj);

    Vector3 GetRandomSpawnPosition()
    {
        // 스포너 반경 spawnDistance의 랜덤 위치를 반환
        return transform.position + Random.insideUnitSphere * spawnDistance;
    }
}
