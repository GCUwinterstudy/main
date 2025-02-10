using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingMove : MonoBehaviour
{
    public float speed = 1f;         // 이동 속도
    public float upDistance = 2f;    // 위로 이동할 거리
    public float downDistance = 2f;  // 아래로 이동할 거리

    private bool movingUp = true;    // 현재 위로 이동 중이면 true, 아니면 false
    private Vector3 initialPosition; // 시작 위치
    private Transform tf;

    void Awake()
    {
        tf = GetComponent<Transform>();
    }

    void Start()
    {
        // 시작 위치 저장
        initialPosition = tf.position;
    }

    void Update()
    {
        if (movingUp)
        {
            // 위로 이동
            tf.Translate(Vector3.up * speed * Time.deltaTime);
            // 시작 위치에서 upDistance 만큼 올라갔으면 방향 전환
            if (tf.position.y >= initialPosition.y + upDistance)
            {
                movingUp = false;
            }
        }
        else
        {
            // 아래로 이동
            tf.Translate(Vector3.down * speed * Time.deltaTime);
            // 시작 위치에서 downDistance 만큼 내려갔으면 방향 전환
            if (tf.position.y <= initialPosition.y - downDistance)
            {
                movingUp = true;
            }
        }
    }
}
