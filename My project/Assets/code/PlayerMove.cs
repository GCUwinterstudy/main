using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed; //최대 속력 변수 
    Rigidbody2D rigid; //물리이동을 위한 변수 선언 
    SpriteRenderer spriteRenderer; //방향전환을 위한 변수 
    Animator animator; //애니메이터 조작을 위한 변수 
    public float minJumpForce = 20f;  // 최소 점프 강도
    public float maxJumpForce = 30f; // 최대 점프 강도
    public float chargeTime = 2f;    // 최대 차징 시간
    private float currentChargeTime = 0f;  // 현재 차징된 시간
    private bool isCharging = false;       // 점프 버튼이 눌린 상태인지 확인
    private bool isJumping = false; 
    private bool canWalk = true;
    private bool isDown = false; 
    private bool isFall = false;
    private bool isStun = false;
    private float fallTimer = 0f; // 하강 시간 누적용
    public float fallThreshold = 3f; // 몇 초 이상이면 isFall로 판정할지

    private void Awake() {
        
        rigid = GetComponent<Rigidbody2D>(); //변수 초기화 
        spriteRenderer = GetComponent<SpriteRenderer>(); // 초기화 
        animator = GetComponent<Animator>();
    }


    void Update(){
        // 버튼에서 손을 떄는 등의 단발적인 키보드 입력은 FixedUpdate보다 Update에 쓰는게 키보드 입력이 누락될 확률이 낮아짐

        if(canWalk && !isStun){
        //Stop speed 
        if(Input.GetButtonUp("Horizontal")){ // 버튼에서 손을 때는 경우 
            // normalized : 벡터 크기를 1로 만든 상태 (단위벡터 : 크기가 1인 벡터)
            // 벡터는 방향과 크기를 동시에 가지는데 크기(- : 왼 , + : 오)를 구별하기 위하여 단위벡터(1,-1)로 방향을 알수 있도록 단위벡터를 곱함 
            rigid.velocity = new Vector2( 0.5f * rigid.velocity.normalized.x , rigid.velocity.y);
        }

        //Direction Sprite
        if(Input.GetButtonDown("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
        
        //Animation
        if( Mathf.Abs(rigid.velocity.x) < 0.2) //속도가 0 == 멈춤 
            animator.SetBool("isWalking",false); //isWalking 변수 : false 
        else// 이동중
            if(!isJumping) 
                animator.SetBool("isWalking",true);

        // 점프 버튼 입력 처리 (기본적으로 스페이스바 사용)
        if (Input.GetKeyDown(KeyCode.Space) && !(isJumping) && !(isStun))
        {
            StartCharging();
        }

        if (Input.GetKey(KeyCode.Space) && !(isStun))
        {
            ChargeJump();
        }

        if (Input.GetKeyUp(KeyCode.Space) && !(isStun))
        {
            PerformJump();
        }

        Debug.Log(canWalk);
        //Debug.Log(rigid.velocity.x);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = canWalk ? Input.GetAxisRaw("Horizontal") : 0f;
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        

        if(rigid.velocity.x > maxSpeed)  //오른쪽으로 이동 (+) , 최대 속력을 넘으면 
            rigid.velocity= new Vector2(maxSpeed, rigid.velocity.y); //해당 오브젝트의 속력은 maxSpeed 
        else if(rigid.velocity.x < maxSpeed*(-1)) // 왼쪽으로 이동 (-) 
            rigid.velocity =  new Vector2(maxSpeed*(-1), rigid.velocity.y); //y값은 점프의 영향이므로 0으로 제한을 두면 안됨 


        //Landing Paltform
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0,1,0)); //빔을 쏨(디버그는 게임상에서보이지 않음 ) 시작위치, 어디로 쏠지, 빔의 색 

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
        //빔의 시작위치, 빔의 방향 , 1:distance , ( 빔에 맞은 오브젝트를 특정 레이어로 한정 지어야할 때 사용 ) // RaycastHit2D : Ray에 닿은 오브젝트 클래스 
    
        //rayHit는 여러개 맞더라도 처음 맞은 오브젝트의 정보만을 저장(?) 
        if(rigid.velocity.y < 0){ // 뛰어올랐다가 아래로 떨어질 때만 빔을 쏨 
            if(rayHit.collider != null){ //빔을 맞은 오브젝트가 있을때  -> 맞지않으면 collider도 생성되지않음 
                if(rayHit.distance < 0.5f) 
                {
                    animator.SetBool("isJumping",false); //거리가 0.5보다 작아지면 변경
                    isJumping = false;
                    if (isFall)
                    {
                        // 추락 종료 → 기절 시작
                        StartCoroutine(StunRoutine(2f)); // 2초 동안 기절
                        isFall = false; // isFall도 해제
                    }

                    if(!isStun)
                    {
                        canWalk = true;
                    }
                }
            }
        }

        if (isJumping)
        {
            // 상승 중 → velocity.y > 0
            // 하강 중 → velocity.y < 0

            if (rigid.velocity.y < 0 && !isDown)
            {
                // 이제 막 하강으로 전환된 경우
                isDown = true;
                animator.SetTrigger("isDown");
            }
        }
        else
        {
            // 바닥에 있거나 점프 전 상태면 하강이 아님
            isDown = false;
        }

        // ---- (추가) 하강 시간(fallTimer) 로직 ----
        if (isDown)
        {
            // 하강 중이면 하강 시간 누적
            fallTimer += Time.deltaTime; // 혹은 Time.fixedDeltaTime

            if (fallTimer >= fallThreshold && !isFall)
            {
                // 특정 시간 이상 하강 상태에 돌입한 경우 추락 상태로 변경
                isFall = true;
                animator.SetBool("isFall", true);
            }
        }
        else
        {
            // 하강이 종료되거나 아직 하강이 아닌 경우
            fallTimer = 0f;
            isFall = false;
            isDown = false;
            animator.SetBool("isFall", false);
        }
    }

    // 기절(Stun) 상태를 2초 동안 유지하는 코루틴
    IEnumerator StunRoutine(float stunDuration)
    {
        Debug.Log("stuned!");
        isStun = true;   // 기절 상태
        canWalk = false; // 이동 불가
        rigid.velocity = new Vector2(0f, rigid.velocity.y);
        
        animator.SetBool("isStun", true);

        yield return new WaitForSeconds(stunDuration);

        // 2초 뒤 기절 해제
        isStun = false;
        animator.SetBool("isStun", false);
        canWalk = true;
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
        // 차징 시간을 증가시키되, 최대 차징 시간을 넘지 않게 제한
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
            // 왼쪽으로 힘
            rigid.AddForce(Vector2.left * horizontalForce, ForceMode2D.Impulse);
            }
            else if (h > 0)
            {
            // 오른쪽으로 힘
            rigid.AddForce(Vector2.right * horizontalForce, ForceMode2D.Impulse);
            }
            // 차징 비율 계산 (0~1 사이 값)
            float chargeRatio = currentChargeTime / chargeTime;

            // 점프 강도 계산
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeRatio);

            // Rigidbody2D에 점프 힘 추가
            rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);

            // 차징 종료
            isCharging = false;
            currentChargeTime = 0f;
            animator.SetBool("isCharging", false);

            isJumping = true; // 점프 상태로 설정
        }
    }
}