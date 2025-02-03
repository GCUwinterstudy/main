using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerFlipController : MonoBehaviourPunCallbacks
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 로컬 플레이어의 경우 PlayerMove에서 flip을 직접 처리하므로 건너뜁니다.
        if (photonView.IsMine)
            return;

        // 원격 플레이어의 경우, Animator에서 동기화된 "isFlip" 값을 읽어 flipX 적용
        bool flip = animator.GetBool("isFlip");
        spriteRenderer.flipX = flip;
    }
}