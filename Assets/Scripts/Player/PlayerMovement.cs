
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] WeaponManager theWeaponManager;

    private CharacterController theCharacterController;
    private AudioSource theAudio;

    [SerializeField] private AudioClip sound_walk;
    private float nextTimeToFire = 0f;
    private float walkSoundRate;

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
        theAudio = GetComponent<AudioSource>();
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

    }

    private void LateUpdate()
    {
        float distance = Vector3.Distance(gameObject.transform.position, lastPosition);


        if (isGrounded && distance > 0.015f &&  Time.time >= nextTimeToFire)
        {
            if (isRun)
                walkSoundRate = 0.5f;
            else
                walkSoundRate = 1.1f;
            nextTimeToFire = Time.time + walkSoundRate;
            //Debug.Log("걷는소리 재생");
            PlaySE(sound_walk);
        }

        lastPosition = gameObject.transform.position;
    }

    // 달리기 실행
    private void Running()
    {
        isRun = true;
        if(theWeaponManager.currentWeaponType == "GUN")
            WeaponManager.currentWeaponAnim.SetBool("Run", true);
        applySpeed = runSpeed;
    }


    // 달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        if (theWeaponManager.currentWeaponType == "GUN")
            WeaponManager.currentWeaponAnim.SetBool("Run", false);
        applySpeed = walkSpeed;
    }

    public bool GetRun()
    {
        return isRun;
    }

    private void PlaySE(AudioClip clip)
    {
        theAudio.clip = clip;
        theAudio.Play();
    }
}
