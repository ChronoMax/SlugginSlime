using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Text playerCountText;
    int joinedplayers = 1, readyplayers;

    [SerializeField]
    Text playerReadyText;

    [SerializeField]
    Text readyBttnText;

    PhotonView photonView;
    bool ready;

    private void Start()
    {
        photonView = PhotonView.Get(this);
        playerCountText.text = joinedplayers + "/4 players joined";
        playerReadyText.text = readyplayers + "/" + joinedplayers + "players are ready";
        photonView.RPC("UpdateText", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        joinedplayers--;
        readyplayers = 0;
        photonView.RPC("UpdateReadyPlayers", RpcTarget.All);
        photonView.RPC("UpdateText", RpcTarget.All);
    }

    public void OnReadyButtonClicked()
    {
        if (!ready)
        {
            photonView.RPC("AddReadyPlayers", RpcTarget.AllBuffered);
            readyBttnText.text = "Unready";
            ready = true;
        }
        else if (ready)
        {
            photonView.RPC("RemoveReadyPlayers", RpcTarget.AllBuffered);
            readyBttnText.text = "Ready";
            ready = false;
        }
    }
    
    [PunRPC]
    int AddReadyPlayers()
    {
        readyplayers++;
        photonView.RPC("UpdateText", RpcTarget.All);
        return readyplayers;
    }

    [PunRPC]
    int RemoveReadyPlayers()
    {
        readyplayers--;
        photonView.RPC("UpdateText", RpcTarget.All);
        return readyplayers;
    }

    [PunRPC]
    int UpdateReadyPlayers()
    {
        if (ready)
        {
            photonView.RPC("AddReadyPlayers", RpcTarget.All);
        }
        return readyplayers;
    }

    [PunRPC]
    void UpdateText()
    {
        joinedplayers = PhotonNetwork.CurrentRoom.PlayerCount;
        playerCountText.text = joinedplayers + "/4 players joined";
        playerReadyText.text = readyplayers + "/" + joinedplayers + "players are ready";
    }
}
