using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public Vector3 spawnPosition = new Vector3(0f, 0f, 0f);

    void Start() {
        if (PhotonNetwork.InRoom) {
            PhotonNetwork.Instantiate("Character", spawnPosition, Quaternion.identity);
        }
    }
}
