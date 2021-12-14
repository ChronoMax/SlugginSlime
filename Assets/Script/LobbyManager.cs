using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    Text playerCountText;
    int joinedplayers = 0, readyplayers;

    [SerializeField]
    Text playerReadyText;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCountText.text = joinedplayers + "/4 players joined";
        playerReadyText.text = readyplayers + "/" + readyplayers + " players are ready";
    }

    // Update is called once per frame
    void Update()
    {
       joinedplayers =  PhotonNetwork.CountOfPlayersInRooms;
    }

    void updateText()
    {

    }

    int joinedPlayer(int joined)
    {
        return joinedplayers;
    }
}
