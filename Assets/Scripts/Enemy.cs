using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C};
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;   // 공격범위
    public GameObject bullet;       //enemy C type
    public bool isChase;    //Enemy 가 추적을 결정하는 변수
    public bool isAttack;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2f);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    private void Update() {
        if(nav.enabled)
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        

    }

    void FreezeVelocity()
    {
        if(isChase) {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
        
    }

    void Targetting()
    {
        float targetRadius = 0f;
        float targetRange = 0f;

        switch (enemyType) {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 12f;
                break;
            case Type.C:
                targetRadius = 0.5f;
                targetRange = 25f;
                break;
        }

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        if(rayHits.Length > 0 && !isAttack) {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType) {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20f, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20f;

                yield return new WaitForSeconds(2f);
                break;
        }
        
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    void FixedUpdate()
    {
        Targetting();
        FreezeVelocity();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Melee") {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
        }
        else if(other.tag == "Bullet") {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            
            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade){
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0) {
            mat.color = Color.white;
        }
        else {
            mat.color = Color.gray;
            gameObject.layer = 12;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            if (isGrenade) {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3f;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15f, ForceMode.Impulse);
            }
            else {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }

            Destroy(gameObject, 4);
        }

    }
}
