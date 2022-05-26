using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;

    Vector3 moveVec;

    public Animator anim;

    // Start is called before the first frame update
    void Awaik()
    {
        // anim = GetComponentInChildren<Animator>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");

        moveVec = new Vector3(hAxis, 0f, vAxis).normalized;

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) *Time.deltaTime;
        // Debug.Log("**" + anim);
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);

        // 케릭터가 나아가는 방향으로 바라보기 구현
        transform.LookAt(transform.position + moveVec);
    }
}
