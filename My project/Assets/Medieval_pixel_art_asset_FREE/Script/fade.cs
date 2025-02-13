using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fade : MonoBehaviour
{
    public float respawnTime = 2.0f; // �ٽ� ��Ÿ��������� �ð�
    public float waitTime = 1.0f; //������� ������ �ð�
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
        isCoroutineRunning = true; //�ڷ�ƾ Ȱ��ȭ

        yield return new WaitForSeconds(respawnTime);

        spriteRenderer.enabled = false; //���� �Ⱥ��̰�
        platformCollider.enabled = false; //���� ��Ȱ��ȭ

        yield return new WaitForSeconds(respawnTime); //respawnTime ��ŭ ���

        spriteRenderer.enabled = true; //���� ���̰�
        platformCollider.enabled = true; // ���� Ȱ��ȭ

        isCoroutineRunning = false; //�ڷ�ƾ ��Ȱ��ȭ
    }
}
