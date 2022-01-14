using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    string LevelToLoad;

    [SerializeField]
    Text playerCountText;
    int joinedplayers = 1, readyplayers, numberOfPlayers;

    [SerializeField]
    Text playerReadyText;

    [SerializeField]
    Text readyBttnText;

    [SerializeField]
    Button startBtn;

    [SerializeField]
    GameObject startBtnGameObject;

    PhotonView photonView;
    bool ready, startGame = false;

    [SerializeField]
    List<Transform> spawnpoints;
    [SerializeField]
    List<bool> playerSlot;
    int selectedSlot;

    //This region is responsible for how the text adapts to the amount of players in the game.
    //Also responsible for the ready/unready system.
    #region Text and Ready behaviour
    private void Start()
    {
        CheckForMasterClient();
        photonView = PhotonView.Get(this);
        playerCountText.text = joinedplayers + "/4 players joined";
        playerReadyText.text = readyplayers + "/" + joinedplayers + "players are ready";
        photonView.RPC("UpdateText", RpcTarget.All);
        StartCoroutine(SelectedSlot());

        StartCoroutine(SpawnPlayerInTube());
    }

    //When a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        joinedplayers--;
        readyplayers = 0;
        UpdateReadyPlayers();
        photonView.RPC("UpdateText", RpcTarget.All);
    }

    //When the ready button is clicked, the follwing will happen
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
    
    //Adding player to the ready int
    [PunRPC]
    int AddReadyPlayers()
    {
        readyplayers++;
        photonView.RPC("UpdateText", RpcTarget.All);
        return readyplayers;
    }

    //Removing a player from the ready int
    [PunRPC]
    int RemoveReadyPlayers()
    {
        readyplayers--;
        photonView.RPC("UpdateText", RpcTarget.All);
        return readyplayers;
    }

    //updating the ready int
    int UpdateReadyPlayers()
    {
        if (ready)
        {
            photonView.RPC("AddReadyPlayers", RpcTarget.All);
        }
        return readyplayers;
    }

    //Updating the text for all clients
    [PunRPC]
    void UpdateText()
    {
        CheckForMasterClient();
        joinedplayers = PhotonNetwork.CurrentRoom.PlayerCount;
        playerCountText.text = joinedplayers + "/4 players joined";
        playerReadyText.text = readyplayers + "/" + joinedplayers + "players are ready";

        if (PhotonNetwork.IsMasterClient && readyplayers >= 2 && readyplayers == joinedplayers)
        {
            startGame = true;
            startBtn.interactable = true;
        }
        else if (PhotonNetwork.IsMasterClient && readyplayers < 2 || readyplayers < joinedplayers)
        {
            startGame = false;
            startBtn.interactable = false;
        }

        if (joinedplayers < 4)
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
            print("Room is open");
        }

        if (joinedplayers == 4)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            print("Room is closed");
        }
    }
    #endregion

    //Region responsible for the player spawns.
    #region SpawnSlot behavior
    IEnumerator SelectedSlot()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < playerSlot.Count; i++)
        {
            if (!playerSlot[i])
            {
                selectedSlot = i;
                playerSlot[i] = true;
                break;
            }
        }
        //print("returned selected int");
        photonView.RPC("senseSelectedSlot", RpcTarget.AllBuffered, selectedSlot);
    }

    [PunRPC]
    void senseSelectedSlot(int slotNumber)
    {
        //print(slotNumber + "NewPlayerSlot");
        playerSlot[slotNumber] = true;
    }

    IEnumerator SpawnPlayerInTube()
    {
        yield return new WaitForSeconds(1);
        //print(selectedSlot);
        int _spawnpoint = selectedSlot;

            PhotonNetwork.Instantiate("Player", new Vector3(
            spawnpoints[_spawnpoint].position.x,
            spawnpoints[_spawnpoint].position.y,
            spawnpoints[_spawnpoint].position.z),
            Quaternion.identity);
    }
    #endregion

    //Region responsible for starting the game.
    #region StartGame behavior
    public void OnClickedStartGame()
    {
        if (startGame)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            photonView.RPC("LoadLevel", RpcTarget.All);
        }
    }

    [PunRPC]
    void LoadLevel()
    {
        PhotonNetwork.LoadLevel(LevelToLoad);
    }

    void CheckForMasterClient()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startBtnGameObject.SetActive(true);
        }
    }
    #endregion
}
