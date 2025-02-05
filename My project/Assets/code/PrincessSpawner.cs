using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public GameObject princessPrefab;
    public Transform spawnPoint;
    void Start()
    {
        // 생성할 위치와 회전값 설정
        // spawnPoint가 할당되어 있다면 해당 위치와 회전을 사용, 그렇지 않으면 기본값 사용
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        // PhotonNetwork.Instantiate를 사용해 네트워크 상에서 프리팹 생성
        PhotonNetwork.Instantiate(princessPrefab.name, spawnPosition, spawnRotation, 0);
    }
}
