using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class MatchMaking : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    [SerializeField] string onlineScene;
    [SerializeField] Text debugText;
    [SerializeField] InputField roomName;

    [Header("Buttons")]
    [SerializeField] Button createButton;
    [SerializeField] Button joinButton;

    void Start()
    {
        debugText.text = "Connecting to servers...";
        PhotonNetwork.ConnectUsingSettings();
    }
}