using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    Transform[] spawnPoints = new Transform[6];

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints[0] = transform.GetChild(0);
        spawnPoints[1] = transform.GetChild(1);
        spawnPoints[2] = transform.GetChild(2);
        spawnPoints[3] = transform.GetChild(3);
        spawnPoints[4] = transform.GetChild(4);
        spawnPoints[5] = transform.GetChild(5);

        SpawnPlayer(Random.Range(0, spawnPoints.Length));
        
        //GetComponent<PhotonView>().RPC("SpawnPlayer");
    }

    private void SpawnPlayer(int index)
    {
        SlimeController slimeScript = PhotonNetwork.Instantiate("CharacterModel", Vector3.up + spawnPoints[index].position, Quaternion.identity).GetComponent<SlimeController>();
        slimeScript.cam = Camera.main.transform;
        GameManager.Instance.PlacePLayerName();
    }
}
