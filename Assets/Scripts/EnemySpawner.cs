using System.Collections;
using UnityEngine;
using UnityEngine.Pool; // ������Ʈ Ǯ���� ���� ���̺귯��

public class EnemySpawner : MonoBehaviour
{
    [Header("�÷��̾� Ʈ������ ����")]
    [SerializeField] private Transform playerTransform;
    [Header("�� ������ ����")]
    [SerializeField] private GameObject zombiePrefab; // ������ ���� ������
    [Header("���� ���� ����")]
    [SerializeField] private float spawnInterval = 2.0f; // ���� ���� ����(��)
    [SerializeField] private float spawnDistance = 3.0f; // �����ʷκ��� ���� ���� �Ÿ�
    [Header("Ǯ ���� ����")]
    [SerializeField] private int initPoolSize;
    [SerializeField] private int maxPoolSize;
    private ObjectPool<GameObject> zombiePool;
    private Coroutine spawnCoroutine;
    

    void Awake()
    {
        //������ �� ������Ʈ Ǯ ����
        zombiePool = new ObjectPool<GameObject>(
            CreateZombie, //Ǯ�� ���̻� ������Ʈ�� ���� �� �� ������Ʈ ����
            OnGetZombie, //Ǯ���� ������ ������Ʈ ��ġ ����
            OnReleaseZombie, //������Ʈ�� Ǯ�� �ݳ��� �� ����
            OnDestroyZombie, //������Ʈ�� ������ �ı����� �� ����
            true,   // �ߺ� ��ȯ üũ
            initPoolSize,     // ���� ���� ����
            maxPoolSize      // �ִ� Ǯ ũ��
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
            // ���� ������ �ϳ� �������� 
            var zombie = zombiePool.Get();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    GameObject CreateZombie()
    {
        var obj = Instantiate(zombiePrefab);
        obj.SetActive(false);

        obj.GetComponent<Zombie>().SetPool(zombiePool);
        //������ �����鿡 �÷��̾� ��ġ ����
        obj.GetComponent<Zombie>().SetPlayerTransform(playerTransform);

        return obj;
    }

    void OnGetZombie(GameObject obj)
    {
        obj.SetActive(true);
        obj.transform.position = GetRandomSpawnPosition();
        //ü��, ���� ���� �ڵ� �ʿ�
    }

    void OnReleaseZombie(GameObject obj) => obj.SetActive(false);

    void OnDestroyZombie(GameObject obj) => Destroy(obj);

    Vector3 GetRandomSpawnPosition()
    {
        // ������ �ݰ� spawnDistance�� ���� ��ġ�� ��ȯ
        return transform.position + Random.insideUnitSphere * spawnDistance;
    }
}
