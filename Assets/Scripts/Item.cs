using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type {
        Ammo, Coin, Grenade, Heart, Weapon
    };
    public Type type;
    public int value;
    
    Rigidbody rigid;
    SphereCollider sphereCollier;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        sphereCollier = GetComponent<SphereCollider>();
    }

    void Update() {
        transform.Rotate(Vector3.up * 10f * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Floor") {
            rigid.isKinematic = true;
            sphereCollier.enabled = false;
        }
    }
}
