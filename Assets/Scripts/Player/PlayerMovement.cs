
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] WeaponManager theWeaponManager;
    private CharacterController theCharacterController;

    [Header("발소리")]
    [SerializeField] private string footstepName = "FootStep"; // SoundManager.effectSounds에 등록한 이름
    [SerializeField] private float walkInterval = 0.5f;        // 걷기 간격
    [SerializeField] private float runInterval = 0.33f;        // 달리기 간격
    [SerializeField] private float minSpeedForStep = 0.1f;     // 임계 속도
    private float stepTimer;

    [Header("플레이어 이동")]
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
            int idx = Random.Range(0, 2);
            string name = (idx == 1) ? "PlayerJump1" : "PlayerJump2";
            SoundManager.instance?.PlaySE(name);
        }

        //중력
        velocity.y += gravity * Time.deltaTime;

        //y축 움직임 적용
        theCharacterController.Move(velocity * Time.deltaTime);


        applySpeed = walkSpeed;

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

        //발소리 재생
        PlayFootsteps();
    }
    private void PlayFootsteps()
    {
        Vector3 hv = theCharacterController.velocity; hv.y = 0f;
        float speed = hv.magnitude; // 현재 이동 속도

        if (!isGrounded || speed < minSpeedForStep)
        {//공중이거나 속도가 임계속도보다 낮을경우 발소리 즉시 중지
            SoundManager.instance?.StopSE(footstepName); 
            return;
        }

        stepTimer -= Time.deltaTime;
        float interval = isRun ? runInterval : walkInterval;

        if (stepTimer <= 0f)
        {
            SoundManager.instance?.PlaySE(footstepName); // 재생
            stepTimer = interval;
        }
    }
    // 달리기 실행
    private void Running()
    {
        isRun = true;
        if (theWeaponManager.currentWeaponType == "GUN")
            WeaponManager.currentWeaponAnim.SetBool("Run", true);
        applySpeed = runSpeed;
    }


    // 달리기 취소
    private void RunningCancel()
    {
        if (theWeaponManager.currentWeaponType == "GUN")
            WeaponManager.currentWeaponAnim.SetBool("Run", false);
        isRun = false;
        applySpeed = walkSpeed;
    }

    public bool GetRun()
    {
        return isRun;
    }
}
