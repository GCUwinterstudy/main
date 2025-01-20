using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class dropStep : MonoBehaviour
{
    public float speed = 3.0f;       // 좌우 이동 속도
    public float distance = 13.0f;   // 좌우 이동 거리

    // 떨어질 때 사용하는 변수들
    public float fallDistance = 100.0f; // 100 단위 만큼 떨어지기 위한 거리
    public float fallSpeed = 5.0f;      // 떨어지는 속도

    private float startPositionX;
    private float rightPositionX;
    private float leftPositionX;

    private bool movingLeft = true;

    // 떨어짐 관련 상태 변수
    private bool isFalling = false;
    private float targetFallY = -29.0f;
    // Start is called before the first frame update
    void Start()
    {
        startPositionX = transform.position.x;
        rightPositionX = startPositionX; // 오른쪽 기준점 (처음 위치)
        leftPositionX = startPositionX - distance;
    }

    // Update is called once per frame
    void Update()
    {
        // 발판이 떨어지는 상태라면 좌우 이동 대신 아래로 이동
        if (isFalling)
        {
            // 목표 위치(targetFallY)보다 아래로 아직 떨어지지 않았다면 계속 떨어지기
            if (transform.position.y > targetFallY)
            {
                transform.Translate(0, -fallSpeed * Time.deltaTime, 0);
            }
            return; // 떨어지는 상태일 때는 더 이상의 좌우 이동 로직 수행하지 않음.
        }

        // 좌우 이동
        if (movingLeft)
        {
            if (transform.position.x > leftPositionX)
            {
                transform.Translate(-speed * Time.deltaTime, 0, 0);
            }
            else
            {
                movingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightPositionX)
            {
                transform.Translate(speed * Time.deltaTime, 0, 0);
            }
            else
            {
                movingLeft = true;
            }
        }
    }
}
