using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    public float maxSpeed; // 최대 속력
    Rigidbody2D rigid;           // 물리 이동을 위한 변수
    SpriteRenderer spriteRenderer; // 방향 전환을 위한 변수
    Animator animator;           // 애니메이터 조작을 위한 변수
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

    // 각 플레이어별 stun 횟수와 jump 횟수
    public int stunCount = 0;
    public int jumpCount = 0;

    // 현재 플레이어가 접촉 중인 포탈
    private GameObject currentTeleporter;
    // 현재 플레이어가 접촉 중인 이동발판
    private moving currentPlatform;


    // 효과음
    public AudioClip jumpSound;
    public AudioClip stunSound;
    public AudioClip chargeSound;
    private AudioSource audioSource;

    private void Awake()
    {
        // 오프라인 모드 설정 (싱글플레이)
        if (!PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.OfflineMode = true;
            Debug.Log("PhotonNetwork OfflineMode 활성화 (싱글플레이 모드)");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
            return;

        // 이동 및 방향 처리 (canWalk와 isStun 상태 확인)
        if (canWalk && !isStun)
        {
            if (Input.GetButtonUp("Horizontal"))
            {
                rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y);
            }

            if (Input.GetButtonDown("Horizontal"))
            {
                float hInput = Input.GetAxisRaw("Horizontal");
                if (hInput < 0)
                {
                    spriteRenderer.flipX = true;
                    animator.SetBool("isFlip", true);
                }
                else if (hInput > 0)
                {
                    spriteRenderer.flipX = false;
                    animator.SetBool("isFlip", false);
                }
            }
        }

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

        //움직이는 발판 따라가기
        if (currentPlatform != null)
        {
            transform.position += currentPlatform.DeltaPosition;
        }

        // 포탈(teleporter) 처리 (E 키 입력)
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentTeleporter != null)
            {
                Transform dest = currentTeleporter.GetComponent<Portal>().GetDestination();
                if (dest != null)
                {
                    Debug.Log("Teleporting to: " + dest.name + " / " + dest.position);
                    transform.position = dest.position;
                }
                else
                {
                    Debug.LogWarning("Destination is not assigned!");
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
            return;

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
                StartCoroutine(StunRoutine(2f)); // 2초 동안 기절
                isFall = false;
            }
            if (!isJumping && !isCharging && !isDown && !isStun)
            {
                canWalk = true;
            }
        }
        else
        {
            canWalk = false;
        }

        if (rigid.velocity.y < 0 && !isDown)
        {
            isDown = true;
            animator.SetTrigger("isDown");
        }

        if (isDown)
        {
            fallTimer += Time.fixedDeltaTime;
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

    // 기절 상태를 지정 시간 동안 유지하는 코루틴 (stun 시 custom property 업데이트)
    IEnumerator StunRoutine(float stunDuration)
    {
        stunCount++;
        Debug.Log("stuned!");
        isStun = true;
        canWalk = false;
        rigid.velocity = new Vector2(0f, rigid.velocity.y);
        animator.SetBool("isStun", true);

        if (stunSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(stunSound);
        }

        yield return new WaitForSeconds(stunDuration);

        isStun = false;
        animator.SetBool("isStun", false);
        if (IsGrounded())
        {
            canWalk = true;
        }
        UpdatePlayerStatsCustomProperties();
    }

    void StartCharging()
    {
        isCharging = true;
        animator.SetBool("isCharging", true);
        currentChargeTime = 0f;
        canWalk = false;

        if (chargeSound != null && audioSource != null) {
            audioSource.loop = true;
            audioSource.clip = chargeSound;
            audioSource.Play();
        }
    }

    void ChargeJump()
    {
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0f, chargeTime);
        }
    }

    void PerformJump()
    {
        if (isCharging)
        {
            if (audioSource != null && audioSource.isPlaying && audioSource.clip == chargeSound)
            {
                audioSource.Stop();
                audioSource.loop = false;
                audioSource.clip = null;
            }
            float horizontalForce = 5f;
            float h = Input.GetAxisRaw("Horizontal");
            if (h < 0)
            {
                rigid.AddForce(Vector2.left * horizontalForce, ForceMode2D.Impulse);
            }
            else if (h > 0)
            {
                rigid.AddForce(Vector2.right * horizontalForce, ForceMode2D.Impulse);
            }
            float chargeRatio = currentChargeTime / chargeTime;
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeRatio);
            rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
            isCharging = false;
            currentChargeTime = 0f;
            animator.SetBool("isCharging", false);
            isJumping = true;
            jumpCount++;
            UpdatePlayerStatsCustomProperties();

            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    // Custom Properties 업데이트 함수 (stunCount, jumpCount, yDistance)
    void UpdatePlayerStatsCustomProperties()
    {
        PhotonHashtable props = new PhotonHashtable();
        props["stunCount"] = stunCount;
        props["jumpCount"] = jumpCount;
        // 기준점은 EndingManager.referenceY (없으면 0으로 가정)
        float referenceY = EndingManager.referenceY;
        float yDistance = transform.position.y - referenceY;
        props["yDistance"] = yDistance;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    // 지면 감지 함수
    bool IsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(
            col2D.bounds.center,
            col2D.bounds.size * 0.9f,
            0f,
            Vector2.down,
            0.2f,
            LayerMask.GetMask("Platform")
        );
        RaycastHit2D rayHit2 = Physics2D.Raycast(
            col2D.bounds.center,
            Vector2.down,
            0.2f,
            LayerMask.GetMask("Platform")
        );
        Debug.DrawRay(col2D.bounds.center, Vector2.down * 0.2f, Color.red);
        return (rayHit.collider != null || rayHit2.collider != null);
    }

    // 지형지물 진입/탈출 처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //포탈(teleporter) 진입 처리
        if (collision.CompareTag("portal"))
        {
            currentTeleporter = collision.gameObject;
        }
        //움직이는 발판 진입 처리
        if (collision.gameObject.CompareTag("movingPlatform"))
        {
            currentPlatform = collision.gameObject.GetComponent<moving>();
            Debug.Log("collision");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //포탈(teleporter) 탈출 처리
        if (collision.CompareTag("portal"))
        {
            if (collision.gameObject == currentTeleporter)
                currentTeleporter = null;
        }
        //움직이는 발판 탈출 처리
        if (collision.gameObject.CompareTag("movingPlatform"))
        {
            currentPlatform = null;
            Debug.Log("release");
        }
    }
}
