using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fade : MonoBehaviour
{
    public float respawnTime = 2.0f; // 다시 나타나기까지의 시간
    public float waitTime = 1.0f; //사라지기 전까지 시간
    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    GameObject player;
    private bool isCoroutineRunning = false;

    

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCoroutineRunning)
        {
            Debug.Log("Trigger");
            player = collision.gameObject;
            StartCoroutine(DisappearAndRespawn());
        }
    }

    IEnumerator DisappearAndRespawn()
    {
        Debug.Log("Coroutine");
        isCoroutineRunning = true; //코루틴 활성화

        yield return new WaitForSeconds(respawnTime);

        spriteRenderer.enabled = false; //발판 안보이게
        platformCollider.enabled = false; //발판 비활성화

        yield return new WaitForSeconds(respawnTime); //respawnTime 만큼 대기

        spriteRenderer.enabled = true; //발판 보이게
        platformCollider.enabled = true; // 발판 활성화

        isCoroutineRunning = false; //코루틴 비활성화
    }
}
