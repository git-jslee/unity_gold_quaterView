using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // 근접(melee) 원거리(range) 공격 type 지정
    public enum Type { Melee, Range};
    public Type type;
    public int damage;
    public float rate;      //공격속도
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;   //공격범위
    public TrailRenderer trailEffect;   //공격효과
    public Transform bulletPos;     // 총알 프리팹 가져올 위치
    public GameObject bullet;       // 총알 프리팹
    public Transform bulletCasePos;     // 탄피 프리팹 가져올 위치
    public GameObject bulletCase;       // 탄피 프리팹

    public void Use()
    {
        if(type == Type.Melee) {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if(type == Type.Range && curAmmo > 0) {
            curAmmo --;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        // 1. 총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50f;

        yield return null;
        // 2. 탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2,3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 탄피 회전
    }
}
