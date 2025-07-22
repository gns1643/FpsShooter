using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("�ʿ��� ������Ʈ��")]
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
          
            anim.SetBool("Running", true); //��� �߰� �ʿ� : �÷��̾���� �Ÿ������� �ѱ�, �ٱ� ��ȯ
        }
        else
        {
            Debug.Log("���� ���� ��ǥ�� ����");
        }
    }
}
