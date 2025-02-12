using System.Collections;
using Photon.Pun;
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
            StartCoroutine(EndRoutine(2.0f));
        }
    }

    IEnumerator EndRoutine(float waitTime)
    {
        Debug.Log("test");

        // 먼저, 모든 클라이언트의 카메라 시점을 전환하기 위해 현재 플레이어의 ViewID 전달
        photonView.RPC("TriggerEndingCameraRPC", RpcTarget.All, photonView.ViewID);

        // 엔딩 스프라이트와 애니메이션 적용
        spriteRenderer.sprite = sprite1;
        animator.SetTrigger("PrincessWalk");

        // 플레이어 이동을 막음 (PlayerMove가 있을 경우)
        PlayerMove playerMove = GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.canWalk = false;
        }

        isWalking = true;

        // 대기 후, 모든 클라이언트에 엔딩 상태를 활성화하도록 RPC 호출
        yield return new WaitForSeconds(waitTime);
        photonView.RPC("TriggerEndingManagerRPC", RpcTarget.All);
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
}
