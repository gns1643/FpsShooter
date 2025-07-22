using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("필요한 컴포넌트들")]
    [SerializeField] Transform thePlayer;
    [SerializeField] Animator anim;
    NavMeshAgent nav;
    void Start()
    {
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
          
            anim.SetBool("Running", true); //기능 추가 필요 : 플레이어와의 거리에따라 겉기, 뛰기 전환
        }
        else
        {
            Debug.Log("좀비가 쫓을 목표가 없음");
        }
    }
}
