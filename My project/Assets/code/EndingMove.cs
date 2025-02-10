using UnityEngine;

public class EndingMove : MonoBehaviour
{
    [Header("Bobbing Settings")]
    [Tooltip("수직 이동 속도 (진동 주파수)")]
    public float verticalSpeed = 2f;
    [Tooltip("수직 이동 진폭 (최대 이동 거리)")]
    public float verticalAmplitude = 2f;

    [Header("Horizontal Drift Settings")]
    [Tooltip("수평 이동 속도")]
    public float horizontalSpeed = 1f;
    [Tooltip("수평 이동 진폭")]
    public float horizontalAmplitude = 1f;

    [Header("Rotation Settings")]
    [Tooltip("회전 진폭 (최대 회전 각도)")]
    public float rotationAmplitude = 5f;

    private Vector3 initialPosition;
    private Transform tf;

    void Awake()
    {
        tf = transform;
    }

    void Start()
    {
        // 시작 위치 저장
        initialPosition = tf.position;
    }

    void Update()
    {
        // 수직 오프셋: 사인 함수를 사용하여 위아래로 진동
        float verticalOffset = Mathf.Sin(Time.time * verticalSpeed) * verticalAmplitude;
        // 수평 오프셋: 코사인 함수를 사용하여 약간의 좌우 흔들림 효과
        float horizontalOffset = Mathf.Cos(Time.time * horizontalSpeed) * horizontalAmplitude;
        // 새로운 위치 계산: 시작 위치를 기준으로 오프셋을 더함
        tf.position = initialPosition + new Vector3(horizontalOffset, verticalOffset, 0f);

        // 회전 효과: 수직 진동에 비례해 약간의 회전(틸트) 효과를 추가
        float rotationAngle = Mathf.Sin(Time.time * verticalSpeed * 2f) * rotationAmplitude;
        tf.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }
}
