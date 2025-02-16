using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] float rightMax = 0f; //좌로 이동가능한 (x)최대값
    [SerializeField] float leftMax = 0f; //우로 이동가능한 (x)최대값
    float currentRow, currentCol; //현재 위치
    float direction = 3.0f; //이동속도+방향

    private Vector3 lastPosition;
    public Vector3 DeltaPosition { get; private set; }

    void Start()

    { 
        currentRow = transform.position.x;
        currentCol = transform.position.y;
        lastPosition = transform.position;
    }

    void Update()
    {
        currentRow += Time.deltaTime * direction;

        //현재 위치(x)가 우로 이동가능한 (x)최대값보다 크거나 같다면
        //이동속도+방향에 -1을 곱해 반전을 해주고 현재위치를 우로 이동가능한 (x)최대값으로 설정
        if (currentRow >= rightMax)
        {
            direction *= -1;
            currentRow = rightMax;
        }

        //현재 위치(x)가 좌로 이동가능한 (x)최대값보다 크거나 같다면
        //이동속도+방향에 -1을 곱해 반전을 해주고 현재위치를 좌로 이동가능한 (x)최대값으로 설정
        else if (currentRow <= leftMax)
        {
            direction *= -1;
            currentRow = leftMax;
        }
        //발판의 위치를 계산된 현재위치로 처리
        transform.position = new Vector3(currentRow, currentCol, 0);

        DeltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;
        //Debug.Log(DeltaPosition + " " + lastPosition);
    }
}