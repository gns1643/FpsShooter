
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController theCharacterController;

    public float walkSpeed;
    public float runSpeed;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    private float applySpeed;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    Vector3 lastPosition = new Vector3(0f,0f,0f);

    bool isGrounded;
    bool isRun = false;

    void Start()
    {
        theCharacterController = GetComponent<CharacterController>();
        applySpeed = walkSpeed;
    }

    void Update()
    {
        //땅에 붙어있는지 체크
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //점프 
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //중력
        velocity.y += gravity * Time.deltaTime;

        //y축 움직임 적용
        theCharacterController.Move(velocity * Time.deltaTime);

        //달리기
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }

        //실제 움직임 적용
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        theCharacterController.Move(move * applySpeed * Time.deltaTime);
      
    }

    // 달리기 실행
    private void Running()
    {
        isRun = true;
        WeaponManager.currentWeaponAnim.SetBool("Run", true);
        applySpeed = runSpeed;
    }


    // 달리기 취소
    private void RunningCancel()
    {
        WeaponManager.currentWeaponAnim.SetBool("Run", false);
        isRun = false;
        applySpeed = walkSpeed;
    }

    public bool GetRun()
    {
        return isRun;
    }
}
