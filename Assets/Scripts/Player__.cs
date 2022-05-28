using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player__ : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;

    Vector3 moveVec;

    Rigidbody rigid;

    Animator anim;

    // Start is called before the first frame update
    void Awaik()
    {
        rigid = GetComponent<Rigidbody>();
        // anim = GetComponentInChildren<Animator>();
        // anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0f, vAxis).normalized;

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) *Time.deltaTime;
        // Debug.Log("**" + anim);
        // anim.SetBool("isRun", moveVec != Vector3.zero);
        // anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // 케릭터가 나아가는 방향으로 바라보기 구현
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if(jDown) {
            rigid.AddForce(Vector3.up * 15f, ForceMode.Impulse);
        }
    }
}
