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
    [SerializeField] GameObject exitPanel;

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
            exitPanel.SetActive(!exitPanel.activeSelf);
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("PhotonMainMenu");
    }

    public void ButtonExitRoom(){
        PhotonNetwork.LeaveRoom();
    }
}
