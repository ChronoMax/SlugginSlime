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
    [SerializeField] Button playButton;
    [SerializeField] Button createButton;
    [SerializeField] Button joinButton;

    void Start()
    {
        debugText.text = "Connecting to servers...";
        PhotonNetwork.ConnectUsingSettings();

        playButton.interactable = false;
        createButton.interactable = false;
    }

    public void SearchGame()
    {
        StartCoroutine(CreateRandomRoom());
    }

    IEnumerator JoinRoom()
    {
        yield return new WaitForSeconds(1);

        PhotonNetwork.JoinRandomRoom();
    }

    IEnumerator CreateRandomRoom()//play / create button
    {
        yield return new WaitForSecondsRealtime(1);

        string randomRoom = Random.Range(1, 100000).ToString();

        RoomOptions roomOpsSpecial = new RoomOptions()
        {
            IsVisible = true, // Private game?
            IsOpen = true, // Joinable?
            MaxPlayers = (byte)4, // RoomSize in Bytes
        };

        if(roomName.text != "")
        {
            debugText.text = $"Joining {roomName.text} game..";
            PhotonNetwork.JoinRoom(roomName.text);
        }
        else
        {
            debugText.text = "Searching for a random game..";
            PhotonNetwork.CreateRoom(randomRoom, roomOpsSpecial);
        }
    }

    //Called when successfully joined a room
    public override void OnJoinedRoom()
    {
        debugText.text = "Joining...";
        PhotonNetwork.LoadLevel(onlineScene);
    }

    //Called when connected to the masterclient
    public override void OnConnectedToMaster()
    {
        debugText.text =
            $"Connected to master server on region " +
            $"{PhotonNetwork.CloudRegion} with ping " +
            $"{PhotonNetwork.GetPing()}";
        playButton.interactable = true;
        createButton.interactable = true;
    }

    //Called when the player has succesfully created a room
    public override void OnCreatedRoom()
    {
        Debug.Log("Created A Room!");
    }

    //Called when there is no room/space to join
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        debugText.text = $"{message}, creating game...";
        StartCoroutine(CreateRandomRoom());
    }
}