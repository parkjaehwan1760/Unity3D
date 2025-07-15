using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("캐릭터")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] Transform character;
    [SerializeField] Animator anicon; // 이미 Animator 참조를 가지고 계시네요!
    [SerializeField] float moveSpeed; // 이동 속도

    [Header("카메라")]
    public Transform camArm;
    public float camAngleSpeed;

    Vector2 moveInput; // 입력받은 이동 방향이 저장될 공간

    [Header("점프")]
    public float jumpPower; // 점프력
    public int MaxJumpCount; // 최대 점프 횟수
    [SerializeField] int nowJumpCount; // 현재 점프 횟수

    bool isJump;

    // --- 여기에 검기 파티클 관련 변수 추가 ---
    [Header("검기 효과")]
    public ParticleSystem mihawkAuraParticle; // 검기 파티클 시스템 연결용

    void Awake()
    {
        nowJumpCount = MaxJumpCount;
        isJump = false;
    }

    void Update()
    {
        Move();
        LookAround();
        Jump();
        Attack(); // Attack 함수 안에서 검기 발동 로직을 추가할 예정
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && 0 < nowJumpCount)
        {
            rigid.velocity = Vector3.up * jumpPower;
            nowJumpCount--;
            isJump = true;
            anicon.SetTrigger("JUMP");
            anicon.SetBool("JUMPEND", false);
        }

        if (rigid.velocity.y <= 0 && Physics.Raycast(character.position + (Vector3.up * 0.1f), Vector3.down, 0.2f, LayerMask.GetMask("Ground")))
        {
            nowJumpCount = MaxJumpCount;
            isJump = false;
            anicon.SetBool("JUMPEND", true);
        }
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
        if (isJump == false)
        {
            anicon.SetBool("ISWALK", moveValue != 0);
        }
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

    [SerializeField] int attackRange;
    [SerializeField] int attackAngle;

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            anicon.SetTrigger("ATTACK"); // 공격 애니메이션 트리거

            // --- 여기에 검기 파티클 발동 로직 추가 ---
            if (mihawkAuraParticle != null)
            {
                mihawkAuraParticle.Stop();  // 혹시 재생 중이던 파티클이 있다면 멈추고
                mihawkAuraParticle.Clear(); // 모든 파티클을 제거한 뒤
                mihawkAuraParticle.Play();  // 새로 재생합니다.
                Debug.Log("Mihawk Slash Particle Activated by X key!"); // 디버그 로그 추가
            }
            else
            {
                Debug.LogWarning("mihawkAuraParticle is not assigned in Player script!");
            }
        }
    }

    public void AttackMonster()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider collider in hitColliders)
        {
            Monster monster = collider.GetComponent<Monster>();
            if (monster != null)
            {
                Vector3 directionToTarget = (monster.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, directionToTarget);

                float angleThreshold = Mathf.Cos(attackAngle * 0.5f * Mathf.Deg2Rad);

                if (dot >= angleThreshold)
                {
                    // 범위 내 몬스터에게 피해
                    monster.Damaged();
                }
            }
        }
    }

    // 공격 범위 시각화 (Scene 뷰에서만 보임)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 forward = transform.forward;
        Quaternion leftRotation = Quaternion.Euler(0, -attackAngle / 2, 0);
        Quaternion rightRotation = Quaternion.Euler(0, attackAngle / 2, 0);

        Vector3 leftDirection = leftRotation * forward;
        Vector3 rightDirection = rightRotation * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftDirection * attackRange);
        Gizmos.DrawLine(transform.position, transform.position + rightDirection * attackRange);
    }
}
