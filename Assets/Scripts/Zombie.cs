using GLTF.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class Zombie : MonoBehaviour
{

    [Header("�ʿ��� ������Ʈ��")]
    [SerializeField] private Animator anim;
    private NavMeshAgent nav;
    private Transform playerTransform;

    [Header("������ ����")]
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
            //�÷��̾���� �Ÿ� ���
            float distance = Vector3.Distance(playerTransform.position, transform.position);

            //�÷��̾���� �Ÿ������� �ٱ�, ����, �ȱ� ��ȯ
            if (distance >= runRange)
                Running();
            else if (distance <= attackRange)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack(); // ���� ���� ����
                    lastAttackTime = Time.time;
                }
                nav.isStopped = true; // ���� �� �̵� ����
            }
            else
                Walking();

        }
        else
        {
            Debug.Log("���� ���� ��ǥ�� ����");
        }
    }
    void Attack()
    {
        anim.SetTrigger("Attack");
        Debug.Log("����!");
        //�÷��̾� ü�� ���� �Լ� ���
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
    { // ���� ������ ȣ��
        this.pool = pool; 
    }
    public void Die()
    { // ���� ����� ȣ��
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
    