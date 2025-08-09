using GLTF.Schema;
using NUnit.Framework.Constraints;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Zombie : MonoBehaviour
{

    [Header("필요한 컴포넌트들")]
    [SerializeField] private Animator anim;
    private NavMeshAgent nav;
    private Transform playerTransform;
    private PlayerStatus playerStat;

    [Header("좀비의 스탯")]
    [SerializeField] int maxHp;
    public int currentHp;
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float wallDetectRange = 2.5f; // 좀비가 벽을 인식할 거리
    private WallStatus targetWall; // 공격 타겟이 벽일 때 저장

    bool isRunning = true;
    float lastAttackTime = -1f;
    private ObjectPool<GameObject> pool;

    private bool isDead;

    private float slowMultiplier = 1f;

    void OnEnable()
    {
        isDead = false;
        currentHp = maxHp;
        nav = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;
        targetWall = FindNearbyWall();
        if (targetWall == null)
        {   //탐지되는 벽이 없다면
            FollowPlayer();
        }
        else
        {   //탐지되는 벽이 있다면
            FollowWall();
        }
        
    }
    void FollowWall()
    {
        float distToWall = Vector3.Distance(transform.position, targetWall.transform.position);

        if (distToWall <= attackRange)
        {
            nav.isStopped = true;
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackWall();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            nav.SetDestination(targetWall.transform.position);
            Walking(); // 또는 달리기
        }
    }
    

    void FollowPlayer()
    {
        if (nav != null)
        {
            nav.SetDestination(playerTransform.position);
            //플레이어와의 거리 계산
            float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);
            //플레이어와의 거리에따라 뛰기, 공격, 걷기 전환
            if (distanceToPlayer >= runRange)
                Running();
            else if (distanceToPlayer <= attackRange)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer(); // 근접 공격 실행
                    lastAttackTime = Time.time;
                }
                nav.isStopped = true; // 공격 중 이동 정지
            }
            else
                Walking();

        }
        else
        {
            Debug.Log("좀비가 쫓을 목표가 없음");
        }
    }
    WallStatus FindNearbyWall()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, wallDetectRange);
        foreach (var hit in hits)
        {
            // "Wall" 태그를 가진 오브젝트만 찾음
            if (hit.CompareTag("Wall"))
            {
                WallStatus wall = hit.GetComponent<WallStatus>();
                if (wall != null && wall.currentHp > 0)
                    return wall;
            }
        }
        return null;
    }
    void AttackWall()
    {
        anim.SetTrigger("Attack");
        Debug.Log("벽 공격!");
        if (targetWall != null)
            targetWall.TakeDamage(10);
    }
    void AttackPlayer()
    {
        anim.SetTrigger("Attack");
        Debug.Log("공격!");
        playerStat.TakeDamage(10);
    }
    void Running()
    {
        if (!isRunning)
        {
            isRunning = true;
            nav.isStopped = false;
            anim.SetBool("Running", true);
            nav.speed = runSpeed;
        }
        
    }
    void Walking()
    {
        if (isRunning)
        {
            isRunning = false;
            nav.isStopped = false;
            nav.speed = walkSpeed;
            anim.SetBool("Running", false);
        }
    }
    public void SetPool(ObjectPool<GameObject> pool) 
    { // 좀비 생성시 호출
        this.pool = pool; 
    }

    IEnumerator  Die()
    { // 좀비 사망시 호출
        isDead = true;
        anim.SetTrigger("Dead");

        yield return new WaitForSeconds(1.5f);
        
        pool.Release(gameObject);
    }

    public void decreaseHp(int m_damage)
    {
        if(currentHp - m_damage > 0)
            currentHp = -m_damage;
        else
        {
            currentHp = 0;
            StartCoroutine(Die());
        }
    }
    public void SetPlayerTransform(Transform m_player, PlayerStatus m_playerStat)
    {
        playerTransform = m_player;
        playerStat = m_playerStat;
    }
    // ★ 감속 배수를 변경 (거미줄 등 느려지는 효과 적용)
    public void SlowDown(float multiplier)
    {
        slowMultiplier = multiplier;
        if (isRunning) ApplySpeed(runSpeed);
        else ApplySpeed(walkSpeed);
    }

    // ★ 원래 속도로 복원
    public void RestoreSpeed()
    {
        slowMultiplier = 1f;
        if (isRunning) ApplySpeed(runSpeed);
        else ApplySpeed(walkSpeed);
    }

    // ★ 기본 속도에 감속 배수 적용
    private void ApplySpeed(float baseSpeed)
    {
        nav.speed = baseSpeed * slowMultiplier;
    }


}
