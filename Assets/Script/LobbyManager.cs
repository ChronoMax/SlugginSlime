using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks

{
    public static LobbyManager Instance;

    [SerializeField]
    Text playerCountText;
    int joinedplayers = 0, readyplayers;

    [SerializeField]
    Text playerReadyText;

    PhotonView photonView;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
        joinedplayers = 1;
        playerCountText.text = joinedplayers + "/4 players joined";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC("SetText", RpcTarget.All);
    }

    [PunRPC]
    void SetText()
    {
        joinedplayers = PhotonNetwork.CurrentRoom.PlayerCount;
        playerCountText.text = joinedplayers + "/4 players joined";
        //playerReadyText.text = readyplayers + "/" + readyplayers + " players are ready";
    }
}
