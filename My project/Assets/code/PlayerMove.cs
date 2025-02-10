using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    public float maxSpeed; // 최대 속력 변수 
    Rigidbody2D rigid;         // 물리 이동을 위한 변수 
    SpriteRenderer spriteRenderer; // 방향 전환을 위한 변수 
    Animator animator;         // 애니메이터 조작을 위한 변수 
    BoxCollider2D col2D;
    
    public float minJumpForce = 20f;  // 최소 점프 강도
    public float maxJumpForce = 30f;  // 최대 점프 강도
    public float chargeTime = 2f;     // 최대 차징 시간
    private float currentChargeTime = 0f;  // 현재 차징된 시간
    private bool isCharging = false;       // 점프 버튼이 눌린 상태 여부
    private bool isJumping = false;
    public bool canWalk = true;
    private bool isDown = false;
    private bool isFall = false;
    private bool isStun = false;
    private float fallTimer = 0f;     // 하강 시간 누적용
    public float fallThreshold = 3f;  // 하강 시간이 이 값 이상이면 추락 상태로 판정
    public int stunCount = 0; // 각 플레이어의 stun 횟수

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (!photonView.IsMine) {
            return;
        }


        // 키 입력은 Update에서 처리 (canWalk와 isStun 상태를 먼저 체크)
        if (canWalk && !isStun)
        {
            // 버튼에서 손을 뗄 때 속도를 약간 감소
            if (Input.GetButtonUp("Horizontal"))
            {
                // normalized: 단위벡터 (방향만 남김)
                rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y);
            }

            // 좌우 입력에 따른 스프라이트 방향 전환 및 isFlip변화
            if (Input.GetButtonDown("Horizontal"))
            {
                float hInput = Input.GetAxisRaw("Horizontal");
                if (hInput < 0) //좌측이동시 true
                {
                    spriteRenderer.flipX = true;
                    animator.SetBool("isFlip", true);
                }
                else if (hInput > 0) //우측이동시 false
                {
                    spriteRenderer.flipX = false;
                    animator.SetBool("isFlip", false);
                }
            }
        }

        // 걷기 애니메이션 처리
        if (Mathf.Abs(rigid.velocity.x) < 0.2f)
            animator.SetBool("isWalking", false);
        else if (!isJumping)
            animator.SetBool("isWalking", true);

        bool isGround = IsGrounded();

        // 점프 입력 처리 (Space 키)
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && !isStun && isGround)
        {
            StartCharging();
        }

        if (Input.GetKey(KeyCode.Space) && !isStun && isGround)
        {
            ChargeJump();
        }

        if (Input.GetKeyUp(KeyCode.Space) && !isStun && isGround)
        {
            PerformJump();
        }

        // 디버그 로그는 개발 시 확인용 (필요 없으면 주석 처리)
        // Debug.Log("isDown : " + isDown);
        // Debug.Log("isGround : " + isGround);
        // Debug.Log("isJumping : " + isJumping);
        // Debug.Log("isFlip : " + isFlip);
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) {
            return;
        }

        bool isGround = IsGrounded();
        float h = canWalk ? Input.GetAxisRaw("Horizontal") : 0f;
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // 최대 속도 제한
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < -maxSpeed)
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);

        if (isGround)
        {
            animator.SetBool("isJumping", false);
            isJumping = false;
            isDown = false;

            if (isFall)
            {
                // 추락 상태 종료 후 착지 시 기절 처리
                StartCoroutine(StunRoutine(2f)); // 2초 동안 기절
                isFall = false;
            }
            // 지면에 착지했을 때, 기절 상태가 아니라면 canWalk를 true로 설정
            if (!isJumping && !isCharging && !isDown && !isStun)
            {
                canWalk = true;
            }
        }
        else
        {
            canWalk = false;
        }

        // 하강 감지: y축 속도가 음수이고 아직 하강 상태가 아니면
        if (rigid.velocity.y < 0 && !isDown)
        {
            isDown = true;
            animator.SetTrigger("isDown");
        }

        // 하강 시간 누적 및 추락 상태 판정
        if (isDown)
        {
            fallTimer += Time.fixedDeltaTime; // FixedUpdate에서는 fixedDeltaTime 사용
            if (fallTimer >= fallThreshold && !isFall)
            {
                isFall = true;
                animator.SetBool("isFall", true);
            }
        }
        else
        {
            fallTimer = 0f;
            isFall = false;
            isDown = false;
            animator.SetBool("isDown", false);
            animator.SetBool("isFall", false);
        }
    }

    // 기절(Stun) 상태를 지정 시간 동안 유지하는 코루틴
    IEnumerator StunRoutine(float stunDuration)
    {
        stunCount ++;
        Debug.Log("stuned!");
        isStun = true;   // 기절 상태
        canWalk = false; // 이동 불가

        /*
        // Custom Properties에 stunCount 업데이트
        Hashtable props = new Hashtable();
        props["stunCount"] = stunCount;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        */
        rigid.velocity = new Vector2(0f, rigid.velocity.y);
        animator.SetBool("isStun", true);

        yield return new WaitForSeconds(stunDuration);

        // 기절 해제 후, 지면에 있다면 이동 가능 상태로 전환
        isStun = false;
        animator.SetBool("isStun", false);
        if (IsGrounded())
        {
            canWalk = true;
        }
    }

    void StartCharging()
    {
        isCharging = true;
        animator.SetBool("isCharging", true);
        currentChargeTime = 0f; // 차징 시간 초기화
        canWalk = false;
    }

    void ChargeJump()
    {
        if (isCharging)
        {
            // 차징 시간을 증가시키되 최대치 초과 방지
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0f, chargeTime);
        }
    }

    void PerformJump()
    {
        if (isCharging)
        {
            float horizontalForce = 5f;
            float h = Input.GetAxisRaw("Horizontal");
            if (h < 0)
            {
                // 왼쪽으로 힘 적용
                rigid.AddForce(Vector2.left * horizontalForce, ForceMode2D.Impulse);
            }
            else if (h > 0)
            {
                // 오른쪽으로 힘 적용
                rigid.AddForce(Vector2.right * horizontalForce, ForceMode2D.Impulse);
            }

            // 차징 비율(0~1)을 계산하여 점프 강도 결정
            float chargeRatio = currentChargeTime / chargeTime;
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeRatio);

            // 점프 실행: y축 속도를 jumpForce로 설정
            rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);

            // 점프 충전 종료
            isCharging = false;
            currentChargeTime = 0f;
            animator.SetBool("isCharging", false);
            isJumping = true; // 점프 상태로 설정
        }
    }

    bool IsGrounded()
    {
        // BoxCast를 사용한 기본 지면 감지
        RaycastHit2D rayHit = Physics2D.BoxCast(
            col2D.bounds.center,
            col2D.bounds.size * 0.9f, // 크기 보정
            0f,
            Vector2.down,
            0.2f,                    // 감지 거리
            LayerMask.GetMask("Platform")
        );

        // 보조 감지: Raycast
        RaycastHit2D rayHit2 = Physics2D.Raycast(
            col2D.bounds.center,
            Vector2.down,
            0.2f,
            LayerMask.GetMask("Platform")
        );

        // 디버깅용 시각화
        Debug.DrawRay(col2D.bounds.center, Vector2.down * 0.2f, Color.red);

        // 두 방식 중 하나라도 충돌하면 지면에 있다고 판단
        return (rayHit.collider != null || rayHit2.collider != null);
    }
}
