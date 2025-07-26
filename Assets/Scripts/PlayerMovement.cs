using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    public float walkSpeed = 4f;
    public float runSpeed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 1.5f;
    public float speed;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Animator anim;

    Vector3 velocity;

    bool isGrounded;
    bool isMoving;
    bool isRun = false;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //���������� Ȯ��
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //������ �Է¹ް� �̵�
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("Run", true);
            isRun = true;
            speed = runSpeed;
        }
        else
        { 
            anim.SetBool("Run", false);
            isRun = false;
            speed = walkSpeed;
        }

        controller.Move(move * speed * Time.deltaTime);

        //����
        if (Input.GetButtonDown("Jump")&&isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity* Time.deltaTime);

        //�����̴� ��Ȳ���� üũ
        if(lastPosition != gameObject.transform.position && isGrounded ==true)
        {
            isMoving = true;
        }
        else
        {
            isMoving= false;
        }    

        lastPosition = gameObject.transform.position;
    }

    public bool GetRun()
    {
        return isRun;
    }
}
