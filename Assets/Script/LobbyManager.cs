using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyManager : MonoBehaviour
{

    [SerializeField] Text RoomName;
    // Start is called before the first frame update
    void Start()
    {
        RoomName.text = PhotonNetwork.CurrentRoom.Name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
