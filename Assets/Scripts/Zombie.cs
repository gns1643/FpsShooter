using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{

    [Header("필요한 컴포넌트들")]
    [SerializeField] private Transform thePlayer;
    [SerializeField] private Animator anim;
    private NavMeshAgent nav;

    [Header("좀비의 스탯")]
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

            //플레이어와의 거리에따라 겉기, 뛰기 전환
            float distance = Vector3.Distance(thePlayer.position, transform.position);
            if (distance >= distanceToRun)
                Running();
            else
                Walking();
        }
        else
        {
            Debug.Log("좀비가 쫓을 목표가 없음");
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
