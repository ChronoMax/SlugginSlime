using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SlimeController slimeScript = PhotonNetwork.Instantiate("CharacterModel", Vector3.up, Quaternion.identity).GetComponent<SlimeController>();
        slimeScript.cam = Camera.main.transform;
    }
}
