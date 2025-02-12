using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    public float bounceForce = 2.0f; // ƨ��� �� ���� ����
    private Rigidbody2D rigid;
    private bool isBouncing = false; // ƨ�� ���� �÷���
    private float bounceDuration = 0.1f; // ƨ�� ���� �ð�

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

   // PlayerBounce.cs

void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Wall") && !isBouncing)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;
            Debug.Log("�浹 ����: " + normal);  // �浹 ���� �α� ���

            if (Mathf.Abs(normal.x) > 0.5f && Mathf.Abs(normal.y) < 0.5f)
            {
                Vector2 currentVelocity = rigid.velocity;
                Vector2 bounceDirection = new Vector2(-currentVelocity.x * 10.0f, 0f);
                
                Debug.Log("bounceDirection = "+ bounceDirection);
                Debug.Log("currentVelocity = "+ currentVelocity);
                // AddForce ������� ƨ���
                rigid.AddForce(bounceDirection, ForceMode2D.Impulse);

                Debug.Log("X������ ƨ����ϴ�!");

                isBouncing = true;
                Invoke("StopBouncing", bounceDuration); // ���� �ð� �� ƨ�� ���� ����
                break;
            }
        }
    }
}


    void StopBouncing()
    {
        isBouncing = false;
    }
}
