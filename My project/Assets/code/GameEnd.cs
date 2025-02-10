using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    SpriteRenderer spriteRenderer; // 방향 전환을 위한 변수 
    Animator animator;         // 애니메이터 조작을 위한 변수 
    Collider2D col2D;
    Rigidbody2D rig;
    Transform tf;
    public Sprite sprite1;
    public float speed = 0.8f;
    private bool isWalking = false;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col2D = GetComponent<Collider2D>();
        rig = GetComponent<Rigidbody2D>();
        tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // isWalking이 true이면 매 프레임 오른쪽으로 일정속도로 이동
        if (isWalking)
        {
            spriteRenderer.flipX = false;
            tf.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Princess"))
        {
            StartCoroutine(EndRoutine(2.0f));
        }
    }

    IEnumerator EndRoutine(float waitTime)
    {
        Debug.Log("test");
        spriteRenderer.sprite = sprite1;
        animator.SetTrigger("PrincessWalk");
        PlayerMove playerMove = GetComponent<PlayerMove>();
        if(playerMove != null)
        {
            playerMove.canWalk = false; //플레이어의 별도 이동 불가
        }
        isWalking = true;
        yield return new WaitForSeconds(waitTime);

        EndingManager.isEnding = true;
        CameraController.isEnding = true;
    }
}
