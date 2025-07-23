using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{

    [Header("�ʿ��� ������Ʈ��")]
    [SerializeField] private Transform thePlayer;
    [SerializeField] private Animator anim;
    private NavMeshAgent nav;

    [Header("������ ����")]
    [SerializeField] int maxHp;
    private int currentHp;
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float distanceToRun;



    void Start()
    {
        currentHp = maxHp;
        nav = gameObject.GetComponent<NavMeshAgent>(); 
    }

    // Update is called once per frame
    void Update()
    {
        FollowingPlayer();
    }

    void FollowingPlayer()
    {
        if (nav != null)
        {
            nav.SetDestination(thePlayer.position);

            //�÷��̾���� �Ÿ������� �ѱ�, �ٱ� ��ȯ
            float distance = Vector3.Distance(thePlayer.position, transform.position);
            if (distance >= distanceToRun)
                Running();
            else
                Walking();
        }
        else
        {
            Debug.Log("���� ���� ��ǥ�� ����");
        }
    }
    void Running()
    {
        anim.SetBool("Running", true);
        nav.speed = runSpeed;
    }
    void Walking()
    {
        anim.SetBool("Running", false);
        nav.speed = walkSpeed;
    }
}
