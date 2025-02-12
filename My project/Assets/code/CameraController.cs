using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviourPunCallbacks
{
    // 엔딩 상태 여부
    public static bool isEnding = false;
    // 공주와 충돌한 플레이어의 PhotonView의 ViewID (RPC로 전달됨)
    public static int targetViewID = -1;
    // 엔딩 상태에서 따라갈 대상 플레이어의 Transform을 찾기 위한 카메라 전환 속도
    public float transitionSpeed = 2f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (isEnding)
        {
            // targetViewID가 유효하면 해당 플레이어 오브젝트를 찾아 카메라를 따라감
            PhotonView targetPV = PhotonView.Find(targetViewID);
            if (targetPV != null)
            {
                Vector3 targetPos = targetPV.transform.position;
                targetPos.z = -10f; // 카메라 z 위치 고정
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
            }
        }
        else
        {
            // 일반 상태: 플레이어를 따라가는 로직 (예시)
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
            }
        }
    }
}
