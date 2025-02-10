using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 엔딩 상태 여부: 엔딩이 시작되면 true로 설정
    public static bool isEnding = false;
    // 엔딩 카메라 위치를 지정할 Transform
    public Transform endingTransform;
    // 카메라가 엔딩 위치로 이동할 때의 전환 속도
    public float transitionSpeed = 2f;

    void Update()
    {
        if (isEnding)
        {

        }
        else
        {
            // 일반 상태에서는 플레이어를 따라감
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -1f);
            }
        }
    }
}
