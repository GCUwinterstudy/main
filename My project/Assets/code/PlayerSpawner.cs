using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public Vector3 spawnPosition = new Vector3(0f, 0f, 0f);
    public float nameYOffset = 0f;

    void Start() 
    {
        GameObject character = null;

        // 멀티플레이일 때는 PhotonNetwork를 이용하여 캐릭터 생성
        if (PhotonNetwork.InRoom)
        {
            character = PhotonNetwork.Instantiate("Character", spawnPosition, Quaternion.identity);
        }
        // 싱글플레이일 경우, Resources 폴더에 있는 "Character" 프리팹을 로드하여 생성
        else
        {
            Object prefab = Resources.Load("Character");
            if (prefab != null)
            {
                character = Instantiate(prefab, spawnPosition, Quaternion.identity) as GameObject;
            }
            else
            {
                Debug.LogError("Character 프리팹을 Resources 폴더에서 찾을 수 없습니다.");
            }
        }

        // 캐릭터가 정상적으로 생성되었다면, 본인이 조종하는 캐릭터에 이름 표시 추가
        if (character != null)
        {
            // 멀티플레이의 경우 PhotonView.IsMine, 싱글플레이의 경우에는 무조건 본인
            bool isMine = PhotonNetwork.InRoom ? character.GetComponent<PhotonView>().IsMine : true;
            if (isMine)
            {
                // 이름 표시용 게임오브젝트 생성 및 부모 설정
                GameObject nameIndicator = new GameObject("MyCharacterName");
                nameIndicator.transform.SetParent(character.transform, false);
                nameIndicator.transform.localPosition = new Vector3(0f, nameYOffset, 0f);

                // TextMeshPro 컴포넌트 추가 후 이름 설정
                TextMeshPro textMesh = nameIndicator.AddComponent<TextMeshPro>();
                string playerName = PhotonNetwork.InRoom ? PhotonNetwork.LocalPlayer.NickName : "Player";
                textMesh.text = playerName;
                textMesh.fontSize = 3f;
                textMesh.alignment = TextAlignmentOptions.Bottom;
                textMesh.margin = new Vector4(0, 0, 0, 3);
                textMesh.color = Color.white;
            }
        }
    }
}