using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameEnd : MonoBehaviourPunCallbacks
{
    SpriteRenderer spriteRenderer; // 방향 전환을 위한 변수 
    Animator animator;             // 애니메이터 조작을 위한 변수 
    Collider2D col2D;
    Rigidbody2D rig;
    Transform tf;
    public Sprite sprite1;
    public float speed = 0.8f;
    private bool isWalking = false;
    public float maxJumpForce = 1f;       // 엔딩 점프 힘

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col2D = GetComponent<BoxCollider2D>();
        rig = GetComponent<Rigidbody2D>();
        tf = GetComponent<Transform>();
    }

    void Update()
    {
        // isWalking이 true이면 매 프레임 오른쪽으로 일정 속도로 이동
        if (isWalking)
        {
            // 항상 오른쪽을 바라보도록 flip 해제
            spriteRenderer.flipX = false;
            tf.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 공주와 충돌 시
        if (other.CompareTag("Princess"))
        {
            string winnerName = PhotonNetwork.LocalPlayer.NickName;
            Debug.Log(winnerName);
            photonView.RPC("SetWinnerRPC", RpcTarget.All, winnerName);
            StartCoroutine(EndRoutine(4.0f));
        }
    }

    IEnumerator EndRoutine(float waitTime)
    {
        Debug.Log("EndRoutine 시작");

        // 모든 클라이언트에서 카메라 전환
        photonView.RPC("TriggerEndingCameraRPC", RpcTarget.All, photonView.ViewID);

        // 모든 클라이언트에서 엔딩 애니메이션(걷기) 실행 및 오른쪽으로 보이도록 설정
        photonView.RPC("SetEndingAnimationRPC", RpcTarget.All);

        isWalking = true;  // 걷기 시작

        // 2초간 걷기
        yield return new WaitForSeconds(1.2f);

        // 모든 클라이언트에서 강제 점프 실행
        photonView.RPC("TriggerEndingJumpRPC", RpcTarget.All);

        // 추가 대기 후 엔딩 상태 활성화 및 플레이어 제거
        yield return new WaitForSeconds(1);
        photonView.RPC("TriggerEndingManagerRPC", RpcTarget.All);
        photonView.RPC("RemovePlayerRPC", RpcTarget.All);
    }

    [PunRPC]
    void TriggerEndingCameraRPC(int viewID)
    {
        // 모든 클라이언트에서 CameraController의 엔딩 상태를 활성화하고, 대상 ViewID를 설정
        CameraController.isEnding = true;
        CameraController.targetViewID = viewID;
    }

    [PunRPC]
    public void TriggerEndingManagerRPC()
    {
        // 모든 클라이언트에서 EndingManager의 엔딩 상태를 활성화
        EndingManager.isEnding = true;
    }

    [PunRPC]
    void SetWinnerRPC(string winnerName)
    {
        EndingManager.winnerName = winnerName;
    }
    
    [PunRPC]
    void RemovePlayerRPC()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void SetEndingAnimationRPC()
    {
        // 모든 클라이언트에서 엔딩 애니메이션 실행
        if (spriteRenderer != null && animator != null)
        {
            spriteRenderer.sprite = sprite1;  // 엔딩 스프라이트 변경
            spriteRenderer.flipX = false;
            animator.SetBool("isFlip", false);
            animator.SetTrigger("PrincessWalk");
        }
    }

    [PunRPC]
    void TriggerEndingJumpRPC()
    {
        ForceJump();
    }

    void ForceJump()
    {
        float jumpForce = maxJumpForce;
        rig.velocity = new Vector2(rig.velocity.x, jumpForce);
    }
}