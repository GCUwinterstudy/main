using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public Vector3 spawnPosition = new Vector3(0f, 0f, 0f);
    // public float indicatorYOffset = 2f;
    public float nameYOffset = 0f;

    void Start() {
        if (PhotonNetwork.InRoom) {
            GameObject character = PhotonNetwork.Instantiate("Character", spawnPosition, Quaternion.identity);

            if (character.GetComponent<PhotonView>().IsMine) {
                // GameObject indicator = new GameObject("MyCharacterIndicator");
                // SpriteRenderer sr = indicator.AddComponent<SpriteRenderer>();
                // sr.sprite = Resources.Load<Sprite>("point");  // Resources 폴더에 있는 "point" 스프라이트
                // sr.sortingLayerName = "UI";
                // sr.sortingOrder = 10;
                // // 캐릭터의 자식으로 설정하고, 오프셋 위치로 배치합니다.
                // indicator.transform.SetParent(character.transform);
                // indicator.transform.localPosition = new Vector3(0f, indicatorYOffset, 0f);



                GameObject nameIndicator = new GameObject("MyCharacterName");
                nameIndicator.transform.SetParent(character.transform, false);
                nameIndicator.transform.localPosition = new Vector3(0f, nameYOffset, 0f);

                TextMeshPro textMesh = nameIndicator.AddComponent<TextMeshPro>();
                textMesh.text = PhotonNetwork.LocalPlayer.NickName;
                textMesh.fontSize = 3f;
                textMesh.alignment = TextAlignmentOptions.Bottom;
                textMesh.margin = new Vector4(0, 0, 0, 3);
                textMesh.color = Color.white;
            }
        }
    }
}