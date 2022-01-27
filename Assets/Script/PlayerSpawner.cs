using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }

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
