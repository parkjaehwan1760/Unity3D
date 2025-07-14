using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("캐릭터")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] Transform character;
    [SerializeField] Animator anicon;
    [SerializeField] float moveSpeed; // 이동 속도

    [Header("카메라")]
    public Transform camArm;
    public float camAngleSpeed;

    Vector2 moveInput; // 입력받은 이동 방향이 저장될 공간

    [Header("점프")]
    public float jumpPower; // 점프력
    public int MaxJumpCount; // 최대 점프 횟수
    [SerializeField] int nowJumpCount; // 현재 점프 횟수

    void Awake()
    {
        nowJumpCount = MaxJumpCount;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && 0 < nowJumpCount)
        {
            rigid.velocity = Vector3.up * jumpPower;
            nowJumpCount--;
        }

        if (rigid.velocity.y <= 0 && Physics.Raycast(character.position + (Vector3.up * 0.1f), Vector3.down, 0.2f, LayerMask.GetMask("Ground")))
        {
            nowJumpCount = MaxJumpCount;
        }
    }

    void Update()
    {
        Move();
        LookAround();
        Jump();
    }

    void Move()
    {
        // 입력
        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput.x = Mathf.MoveTowards(moveInput.x, rawInput.x, Time.deltaTime * 10);
        moveInput.y = Mathf.MoveTowards(moveInput.y, rawInput.y, Time.deltaTime * 10);
        float moveValue = moveInput.magnitude;

        // 이동
        if (moveValue != 0)
        {
            Vector3 lookForward = new Vector3(camArm.forward.x, 0f, camArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(camArm.right.x, 0f, camArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            character.forward = moveDir;

            rigid.MovePosition(character.position + (moveDir * Time.deltaTime * moveSpeed));

            if (moveInput != Vector2.zero)
            {
                Vector3 inputForward = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(inputForward);
                character.rotation = Quaternion.Slerp(character.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        // 애니메이션
        anicon.SetBool("ISWALK", moveValue != 0);
    }

    public void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X") * camAngleSpeed, Input.GetAxis("Mouse Y") * camAngleSpeed);
        Vector3 camAngle = camArm.rotation.eulerAngles;

        float camAngleX = camAngle.x - mouseDelta.y;

        if (camAngleX < 180f)
        {
            camAngleX = Mathf.Clamp(camAngleX, -1f, 70f);
        }
        else
        {
            camAngleX = Mathf.Clamp(camAngleX, 340f, 361f);
        }

        camArm.rotation = Quaternion.Euler(camAngleX, camAngle.y + mouseDelta.x, camAngle.z);
    }
}
