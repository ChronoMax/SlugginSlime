using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    [SerializeField] Text RoomName;
    [SerializeField] GameObject objectToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        RoomName.text = PhotonNetwork.CurrentRoom.Name;

        PhotonNetwork.Instantiate(objectToSpawn.name, new Vector3(0, 0.5f, -10), Quaternion.identity);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    PhotonNetwork.LeaveRoom();
        //}
    }
}
