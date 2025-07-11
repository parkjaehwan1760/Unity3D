using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    [SerializeField] float moveSpeed;
    [SerializeField] Animator anicon_PicoChan;

    void Update()
    {
        // 입력
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // 이동
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 회전
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
           anicon_PicoChan.SetTrigger("ISATTACK");
        }
        // 애니메이터
        bool isWalk = 0 < moveDirection.magnitude;
        // moveDirection.magnitude : 백터의 길이를 반환합니다.
        // 입력 값을 받으면 백터의 길이가 0보다 커지면서 True를 반환합니다.
        anicon_PicoChan.SetBool("ISWALK", isWalk);
        // anicon_PicoChan이라는 애니메이터를 담을 변수를 생성합니다.
        // Bool 타입의 Parameter를 생성하였기에 SetBool함수를 사용합니다.
        anicon_PicoChan.SetBool("ISATTACK", Input.GetKey(KeyCode.Space));
    }
}
