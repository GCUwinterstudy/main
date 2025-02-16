using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class FadePlatform : MonoBehaviour
{
    public float respawnTime = 2.0f; // 다시 나타나기까지의 시간
    public float waitTime = 10.0f; //사라지기 전까지 시간
    private TilemapRenderer tilemapRenderer;
    private Collider2D platformCollider;
    GameObject player;
    private bool isCoroutineRunning;

    

    void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }
    private void Start()
    {
        isCoroutineRunning = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCoroutineRunning)
        {
            StartCoroutine(DisappearAndRespawn());
        }
    }

    IEnumerator DisappearAndRespawn()
    {
        isCoroutineRunning = true; //코루틴 활성화

        yield return new WaitForSeconds(waitTime);

        tilemapRenderer.enabled = false; //발판 안보이게
        platformCollider.enabled = false; //발판 비활성화

        yield return new WaitForSeconds(respawnTime); //respawnTime 만큼 대기

        tilemapRenderer.enabled = true; //발판 보이게
        platformCollider.enabled = true; // 발판 활성화

        isCoroutineRunning = false; //코루틴 비활성화
    }
}
