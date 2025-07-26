using GLTF.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class Zombie : MonoBehaviour
{

    [Header("필요한 컴포넌트들")]
    [SerializeField] private Animator anim;
    private NavMeshAgent nav;
    private Transform playerTransform;

    [Header("좀비의 스탯")]
    [SerializeField] int maxHp;
    private int currentHp;
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;

    float lastAttackTime = -1f;
    private ObjectPool<GameObject> pool;
    
    void Start()
    {
        currentHp = maxHp;
        nav = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }

    void FollowPlayer()
    {
        if (nav != null)
        {
            nav.SetDestination(playerTransform.position);
            //플레이어와의 거리 계산
            float distance = Vector3.Distance(playerTransform.position, transform.position);

            //플레이어와의 거리에따라 뛰기, 공격, 걷기 전환
            if (distance >= runRange)
                Running();
            else if (distance <= attackRange)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack(); // 근접 공격 실행
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
    void Attack()
    {
        anim.SetTrigger("Attack");
        Debug.Log("공격!");
        //플레이어 체력 감소 함수 사용
    }
    void Running()
    {
        nav.isStopped = false;
        anim.SetBool("Running", true);
        nav.speed = runSpeed;
    }
    void Walking()
    {
        nav.isStopped = false;
        anim.SetBool("Running", false);
        nav.speed = walkSpeed;
    }
    public void SetPool(ObjectPool<GameObject> pool) 
    { // 좀비 생성시 호출
        this.pool = pool; 
    }
    public void Die()
    { // 좀비 사망시 호출
        pool.Release(gameObject);
    }

    public void decreaseHp(int m_damage)
    {
        
        if(currentHp - m_damage > 0)
            currentHp = -m_damage;
        else
        {
            currentHp = 0;
            Die();
        }
    }
}
    