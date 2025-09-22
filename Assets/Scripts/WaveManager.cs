using System.Collections;
using TMPro;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public int[] zombieCounts; // [Normal, Fast, Tank] 순서로 수량 저장
}

public class WaveManager : MonoBehaviour
{
    //싱글톤화
    public static WaveManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private WaveData[] waves;
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private TMP_Text waveTMPText;
    [SerializeField] private TMP_Text countDownTMPText;
    [SerializeField] private TMP_Text currentEnemyCount;
    [SerializeField] private int WaveCountDown;
    private int currentWave = 0;
    private int unlockedSpawnerCount = 1; //언락되는 스포너 수(처음엔 1)
    public int totalZombiesToSpawn = 0; // 이번 웨이브에서 소환될 총 좀비 수
    public int zombiesSpawnedCount = 0; // SpawnEnemy 호출된 횟수

    private int aliveZombieCount = 0;

    //필요 컴포넌트
    public PlayerStatus playerStatus;
    public GameObject Item;

    private void Start()
    {
        StartCoroutine(StartWave());
    }

    IEnumerator SpawnWaveRoutine(WaveData data)
    {
        int totalSpawners = unlockedSpawnerCount;
        int zombieTypeCount = data.zombieCounts.Length;

        totalZombiesToSpawn = 0;
        for (int i = 0; i < zombieTypeCount; i++)
            totalZombiesToSpawn += data.zombieCounts[i];

        int[][] perSpawnerCounts = new int[totalSpawners][];
        for (int i = 0; i < totalSpawners; i++)
            perSpawnerCounts[i] = new int[zombieTypeCount];

        for (int type = 0; type < zombieTypeCount; type++)
        {
            int totalCount = data.zombieCounts[type];
            int baseCount = totalCount / totalSpawners;
            int remainder = totalCount % totalSpawners;
            for (int spawner = 0; spawner < totalSpawners; spawner++)
                perSpawnerCounts[spawner][type] = baseCount;
            for (int r = 0; r < remainder; r++)
                perSpawnerCounts[r][type]++;
        }

        for (int spawner = 0; spawner < totalSpawners; spawner++)
        {
            for (int type = 0; type < zombieTypeCount; type++)
            {
                if (perSpawnerCounts[spawner][type] > 0)
                {
                    spawners[spawner].StartAutoSpawn(
                        (EnemySpawner.ZombieType)type,
                        perSpawnerCounts[spawner][type]
                    );
                }
            }
        }
        yield return null; // SpawnWaveRoutine 자체는 소환 시작하기만 함
    }
    private IEnumerator StartWave()
    {
        while (currentWave < waves.Length)
        {
            yield return StartCoroutine(CountdownRoutine());
            UpdateWaveUI();
            Item.SetActive(false);
            currentEnemyCount.gameObject.SetActive(true);

            yield return StartCoroutine(SpawnWaveRoutine(waves[currentWave]));

            // 웨이브 종료 조건 (모든 좀비 사망 대기)
            yield return new WaitUntil(() => aliveZombieCount <= 0 && zombiesSpawnedCount == totalZombiesToSpawn);

            zombiesSpawnedCount = 0; //좀비 스폰 다시 시작할꺼니까 0으로 초기화  
            Item.SetActive(true);
            currentEnemyCount.gameObject.SetActive(false);

            currentWave++;
            UpdateWaveUI();

            UnlockNextSpawner(); //enemyspawnerr추가 개방시 추가 개방하는 함수
        }

        playerStatus.GameEnd();

    }

    IEnumerator CountdownRoutine()
    {
        if (countDownTMPText != null)
        {
            countDownTMPText.gameObject.SetActive(true);

            for (int i = WaveCountDown+2; i > 0; i--)
            {
                if (i == WaveCountDown + 2)
                {
                    SoundManager.instance.PlayBGM("CountDownStart");
                    countDownTMPText.text = "곧 다음 웨이브가 시작합니다.";
                }
                else if(i == 1)
                {
                    SoundManager.instance.PlaySE("CountDownEnd");
                    SoundManager.instance.StopBGM();
                    countDownTMPText.text = "웨이브 시작!!";
                }
                else
                {
                    countDownTMPText.text = (i - 1).ToString();
                }
                yield return new WaitForSeconds(1f);
            }
            countDownTMPText.gameObject.SetActive(false);
        }
        else
        {
            // 카운트다운 텍스트 없으면 바로 리턴
            yield return null;
        }
    }

    void UpdateWaveUI()
    {
        if (waveTMPText != null)
            waveTMPText.text = (currentWave+1) + "번째 웨이브";
    }
    void UnlockNextSpawner()
    {// 3,6 웨이브에서 스포너 하나씩 증가
        if (unlockedSpawnerCount < spawners.Length && (currentWave == 4 || currentWave == 7))
            unlockedSpawnerCount++;
    }
    public void OnZombieSpawned() { aliveZombieCount++; }
    public void OnZombieKilled() { aliveZombieCount--; }

    public void PrintZombieCount()
    { //남은 좀비 수 업데이트
        //Debug.Log("남은 좀비 수 : " + aliveZombieCount);
        currentEnemyCount.text = "현재 남은 좀비 수 : " + aliveZombieCount.ToString();
    }
}
