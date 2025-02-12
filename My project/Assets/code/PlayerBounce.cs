using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    public float bounceForce = 2.0f; // 튕기는 힘 조절 변수
    private Rigidbody2D rigid;
    private bool isBouncing = false; // 튕김 상태 플래그
    private float bounceDuration = 0.1f; // 튕김 유지 시간

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
            Debug.Log("충돌 법선: " + normal);  // 충돌 방향 로그 출력

            if (Mathf.Abs(normal.x) > 0.5f && Mathf.Abs(normal.y) < 0.5f)
            {
                Vector2 currentVelocity = rigid.velocity;
                Vector2 bounceDirection = new Vector2(-currentVelocity.x * 10.0f, 0f);
                
                Debug.Log("bounceDirection = "+ bounceDirection);
                Debug.Log("currentVelocity = "+ currentVelocity);
                // AddForce 방식으로 튕기기
                rigid.AddForce(bounceDirection, ForceMode2D.Impulse);

                Debug.Log("X축으로 튕겼습니다!");

                isBouncing = true;
                Invoke("StopBouncing", bounceDuration); // 일정 시간 후 튕김 상태 해제
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
