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

    Animator anim;

    // Start is called before the first frame update
    void Awaik()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButtonDown("Walk");

        moveVec = new Vector3(hAxis, 0f, vAxis).normalized;

        transform.position += moveVec * speed * Time.deltaTime;
        Debug.Log("**" + anim);
        // anim.SetBool("isRun", moveVec != Vector3.zero);
        // anim.SetBool("isWalk", wDown);
    }
}
