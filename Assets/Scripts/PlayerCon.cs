using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    public float speed;
    // 무기 저장
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public Camera followCamera; //총 발사시 - 카메라 변수 할당.


    // 탄약, 동전, 체력, 수류탄 변수 생성
    public int ammo;
    public int coin;
    public int health;
    
    // 각 수치의 최대값을 저장할 변수 생성
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;    

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    bool fDown;
    bool rDown;     // 총알 재장전
    // 무기 선택 버튼 1, 2, 3
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool iDown; //아디템 획득버튼 'e'
    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;

    Vector3 moveVec;
    // Dodge(회피)중 방향전환 안되게.
    Vector3 dodgeVec;

    Animator anim;

    Rigidbody rigid;
    GameObject nearObject;
    // GameObject equipWeapon;     //장착중인 무기 저정 변수
    Weapon equipWeapon;
    int equipWeaponIndex = -1;       // 현재 들고있는 무기 index
    float fireDelay;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Reload();
        Dodge();
        Swap(); //무기 교체 함수
        Interaction();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0f, vAxis).normalized;

        if(isDodge) moveVec = dodgeVec;

        if(isSwap || isReload || !isFireReady) moveVec = Vector3.zero;

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) *Time.deltaTime;
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);        
    }

    void Turn()
    {
        // 케릭터가 나아가는 방향으로 바라보기 구현
        // 1. 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec);

        // 2. 마우스에 의한 회전
        if (fDown) {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100f)) {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0f;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap) {
            rigid.AddForce(Vector3.up * 15f, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack()
    {
        if(equipWeapon == null) return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if(equipWeapon == null) return;

        if(equipWeapon.type == Weapon.Type.Melee) return;

        if(ammo == 0) return;

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    void Dodge()
    {
        if(jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap) {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            //특정 시간 후 DodgeOut 호출
            Invoke("DodgeOut", 0.4f);
        }
    }

    void Swap()
    {
        if(sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) return;
        if(sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) return;
        if(sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) return;

        int weaponIndex = -1;
        if(sDown1) weaponIndex = 0;
        if(sDown2) weaponIndex = 1;
        if(sDown3) weaponIndex = 2;

        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if(equipWeapon != null) equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        if(iDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon") {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Floor") {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Item") {
            Item item = other.GetComponent<Item>();
            switch(item.type) {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo) ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin) coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth) health = maxHealth;
                    break;
                case Item.Type.Grenade:
                grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades) hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.tag == "Weapon")
        {
            nearObject = other.gameObject;
            // Debug.Log(">>> " + nearObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Weapon")
        {
            nearObject = null;
        }
    }
}
