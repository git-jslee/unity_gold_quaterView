using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    bool jDown;
    Rigidbody rigid;
    // Start is called before the first frame update
    private void Awake() {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        jDown = Input.GetButtonDown("Jump");
        
        if(jDown) {
            rigid.AddForce(Vector3.up * 15f, ForceMode.Impulse);
        }
    }
}
