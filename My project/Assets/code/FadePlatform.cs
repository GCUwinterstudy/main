using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class FadePlatform : MonoBehaviour
{
    public float respawnTime = 2.0f; // �ٽ� ��Ÿ��������� �ð�
    public float waitTime = 10.0f; //������� ������ �ð�
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
        isCoroutineRunning = true; //�ڷ�ƾ Ȱ��ȭ

        yield return new WaitForSeconds(waitTime);

        tilemapRenderer.enabled = false; //���� �Ⱥ��̰�
        platformCollider.enabled = false; //���� ��Ȱ��ȭ

        yield return new WaitForSeconds(respawnTime); //respawnTime ��ŭ ���

        tilemapRenderer.enabled = true; //���� ���̰�
        platformCollider.enabled = true; // ���� Ȱ��ȭ

        isCoroutineRunning = false; //�ڷ�ƾ ��Ȱ��ȭ
    }
}
