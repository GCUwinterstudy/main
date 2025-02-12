using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public Vector3 spawnPosition = new Vector3(0f, 0f, 0f);
    public float nameYOffset = 0f;
    private bool spawned = false;

    void Start() 
    {
        // 만약 PhotonNetwork에 연결되어 있지 않다면(즉, 싱글플레이라면)
        // 오프라인 모드를 활성화하고 임시 방을 생성하여 PhotonNetwork.Instantiate가 사용되도록 합니다.
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom("OfflineRoom", new RoomOptions { MaxPlayers = 1 });
            Debug.Log("OfflineMode 활성화 및 방 생성 (싱글플레이)");
        }
        else
        {
            SpawnCharacter();
        }
    }

    // 방에 가입되었을 때 호출됩니다.
    public override void OnJoinedRoom()
    {
        if (!spawned)
        {
            SpawnCharacter();
        }
    }

    void SpawnCharacter() 
    {
        spawned = true;
        // 온라인이든 오프라인이든 항상 PhotonNetwork.Instantiate를 사용하여 캐릭터 생성
        GameObject character = PhotonNetwork.Instantiate("Character", spawnPosition, Quaternion.identity);

        if (character != null)
        {
            // PhotonNetwork.InRoom가 true인 경우, 멀티플레이라면 PhotonView의 소유권 체크,
            // 오프라인 모드(싱글플레이)라면 무조건 본인으로 간주
            bool isMine = PhotonNetwork.InRoom ? character.GetComponent<PhotonView>().IsMine : true;
            if (isMine)
            {
                // 캐릭터 이름 표시용 게임오브젝트 생성 및 자식으로 설정
                GameObject nameIndicator = new GameObject("MyCharacterName");
                nameIndicator.transform.SetParent(character.transform, false);
                nameIndicator.transform.localPosition = new Vector3(0f, nameYOffset, 0f);

                // TextMeshPro 컴포넌트 추가 및 이름 설정
                TextMeshPro textMesh = nameIndicator.AddComponent<TextMeshPro>();
                // 멀티플레이라면 PhotonNetwork.LocalPlayer.NickName, 싱글플레이라면 기본 문자열 사용
                string playerName = PhotonNetwork.InRoom ? PhotonNetwork.LocalPlayer.NickName : "";
                textMesh.text = playerName;
                textMesh.fontSize = 3f;
                textMesh.alignment = TextAlignmentOptions.Bottom;
                textMesh.margin = new Vector4(0, 0, 0, 3);
                textMesh.color = Color.white;
            }
        }
    }
}