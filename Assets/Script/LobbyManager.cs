using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyManager : MonoBehaviour
{

    [SerializeField] Text RoomName;
    [SerializeField] GameObject objectToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        RoomName.text = PhotonNetwork.CurrentRoom.Name;

        PhotonNetwork.Instantiate(objectToSpawn.name, Vector3.zero, Quaternion.identity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
