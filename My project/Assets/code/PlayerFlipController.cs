using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerFlipController : MonoBehaviourPun
{
    private Animator animator;
    private SpriteRenderer sr;

    private void Awake() {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        // 예: 왼쪽 이동 입력 시 Flip을 true, 오른쪽 이동 시 false로 설정
        bool shouldFlip = Input.GetAxisRaw("Horizontal") < 0;
        animator.SetBool("Flip", shouldFlip);

        // 로컬에서는 Animator의 파라미터에 따라 SpriteRenderer의 flipX를 업데이트
        sr.flipX = animator.GetBool("Flip");
    }
}
