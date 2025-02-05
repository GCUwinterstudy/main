using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PrincessManager : MonoBehaviourPunCallbacks
{
    SpriteRenderer spriteRenderer; // 방향 전환을 위한 변수 
    Animator animator;             // 애니메이터 조작을 위한 변수 
    Collider2D col2D;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col2D = GetComponent<Collider2D>();
    }

    void Update()
    {
        
    }

    // Trigger로 설정된 Collider에 다른 오브젝트가 닿았을 때 호출됨
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player와 접촉함");
            // 모든 클라이언트에 삭제 RPC 호출 (Buffered로 호출하면 새로 접속하는 클라이언트에도 삭제 상태가 전달됨)
            photonView.RPC("DestroyPrincess", RpcTarget.AllBuffered);
        }
    }

    // RPC 메서드: 실제 삭제를 실행 (소유자에서만 PhotonNetwork.Destroy를 호출)
    [PunRPC]
    void DestroyPrincess()
    {
        if(photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
