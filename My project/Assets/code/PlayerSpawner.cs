using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public Vector3 spawnPosition = new Vector3(0f, 0f, 0f);
    public float indicatorYOffset = 1f;

    void Start() {
        if (PhotonNetwork.InRoom) {
            GameObject character = PhotonNetwork.Instantiate("Character", spawnPosition, Quaternion.identity);

            if (character.GetComponent<Photon.Pun.PhotonView>().IsMine) {
                GameObject indicator = new GameObject("MyCharacterIndicator");
                SpriteRenderer sr = indicator.AddComponent<SpriteRenderer>();

                sr.sprite = Resources.Load<Sprite>("point");

                sr.sortingLayerName = "UI";
                sr.sortingOrder = 10;

                indicator.transform.SetParent(character.transform);
                indicator.transform.localPosition = new Vector3(0f, indicatorYOffset, 0f);
            }
            
        }
    }
}
