using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class dropStep : MonoBehaviour
{
    public float speed = 3.0f;       // �¿� �̵� �ӵ�
    public float distance = 13.0f;   // �¿� �̵� �Ÿ�

    // ������ �� ����ϴ� ������
    public float fallDistance = 100.0f; // 100 ���� ��ŭ �������� ���� �Ÿ�
    public float fallSpeed = 5.0f;      // �������� �ӵ�

    private float startPositionX;
    private float rightPositionX;
    private float leftPositionX;

    private bool movingLeft = true;

    // ������ ���� ���� ����
    private bool isFalling = false;
    private float targetFallY = -29.0f;
    // Start is called before the first frame update
    void Start()
    {
        startPositionX = transform.position.x;
        rightPositionX = startPositionX; // ������ ������ (ó�� ��ġ)
        leftPositionX = startPositionX - distance;
    }

    // Update is called once per frame
    void Update()
    {
        // ������ �������� ���¶�� �¿� �̵� ��� �Ʒ��� �̵�
        if (isFalling)
        {
            // ��ǥ ��ġ(targetFallY)���� �Ʒ��� ���� �������� �ʾҴٸ� ��� ��������
            if (transform.position.y > targetFallY)
            {
                transform.Translate(0, -fallSpeed * Time.deltaTime, 0);
            }
            return; // �������� ������ ���� �� �̻��� �¿� �̵� ���� �������� ����.
        }

        // �¿� �̵�
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
