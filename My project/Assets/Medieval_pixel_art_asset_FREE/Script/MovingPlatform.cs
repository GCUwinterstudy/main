using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] float rightMax = 0f; //�·� �̵������� (x)�ִ밪
    [SerializeField] float leftMax = 0f; //��� �̵������� (x)�ִ밪
    float currentRow, currentCol; //���� ��ġ
    float direction = 3.0f; //�̵��ӵ�+����

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

        //���� ��ġ(x)�� ��� �̵������� (x)�ִ밪���� ũ�ų� ���ٸ�
        //�̵��ӵ�+���⿡ -1�� ���� ������ ���ְ� ������ġ�� ��� �̵������� (x)�ִ밪���� ����
        if (currentRow >= rightMax)
        {
            direction *= -1;
            currentRow = rightMax;
        }

        //���� ��ġ(x)�� �·� �̵������� (x)�ִ밪���� ũ�ų� ���ٸ�
        //�̵��ӵ�+���⿡ -1�� ���� ������ ���ְ� ������ġ�� �·� �̵������� (x)�ִ밪���� ����
        else if (currentRow <= leftMax)
        {
            direction *= -1;
            currentRow = leftMax;
        }
        //������ ��ġ�� ���� ������ġ�� ó��
        transform.position = new Vector3(currentRow, currentCol, 0);

        DeltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;
        //Debug.Log(DeltaPosition + " " + lastPosition);
    }
}