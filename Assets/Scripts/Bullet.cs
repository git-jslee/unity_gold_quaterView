using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Floor") {
            Destroy(gameObject, 3f);
        }

    }

    private void OnTriggerEnter(Collider other) {
        // 플레이어가 벽 근처에 있을때..enemy 가 벽으로 인하여 destory 방지
        if(!isMelee && other.gameObject.tag == "Wall") {
            Destroy(gameObject);
        }        
    }

}
