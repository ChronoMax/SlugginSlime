using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class OnlinePlaybutton : MonoBehaviourPunCallbacks
{
    [SerializeField] string onlineScene;

    [SerializeField] Button createButton;

    [SerializeField] Button joinButton;
    [SerializeField] Text debugText;

    [SerializeField] InputField roomName;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        debugText.text = "Connecting to servers...";
    }
    //called from play button
    public void Connecttogame()
    {

        createButton.interactable = false;
    }

    public void ConnectoCreatedgame()
    {
        StartCoroutine(CreateGame());
    }

    void Update()
    {
        if(roomName.text.Length == 5)
        {
            joinButton.interactable = true;
        }
        else
        {
            joinButton.interactable = false;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            Debug.Log(info);
        }
    }

    //Called when connected to the masterclient
    public override void OnConnectedToMaster()
    {
        debugText.text =
            $"Connected to master server on region " +
            $"{PhotonNetwork.CloudRegion} with ping " +
            $"{PhotonNetwork.GetPing().ToString()}";

        //SearchGame();
    }

    void SearchGame()
    {
        if(roomName.text != "")
        {
            PhotonNetwork.JoinRoom(roomName.text);
            debugText.text = $"Joining {roomName.text} game..";
        }
        
        debugText.text = "Searching for a random game..";
        StartCoroutine(SearchCreateGame());
    }

    IEnumerator SearchCreateGame()
    {
        yield return new WaitForSecondsRealtime(1);
        PhotonNetwork.JoinRandomRoom();

        //PhotonNetwork.JoinRandomOrCreateRoom();
    }

    //Called when successfully joined a room
    public override void OnJoinedRoom()
    {
        debugText.text = "Joining...";
        PhotonNetwork.LoadLevel(onlineScene);
    }

    //Called when there is no room/space to join
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        debugText.text = $"{message}, creating game...";
        StartCoroutine(CreateGame());
    }

    IEnumerator CreateGame()
    {
        yield return new WaitForSecondsRealtime(1);

        string roomName = Random.Range(1, 100000).ToString();

        RoomOptions roomOpsSpecial = new RoomOptions()
        {
            IsVisible = true, // Private game?
            IsOpen = true, // Joinable?
            MaxPlayers = (byte)4, // RoomSize in Bytes
        };

        PhotonNetwork.CreateRoom(roomName, roomOpsSpecial);
    }

    //Called when the player has succesfully created a room
    public override void OnCreatedRoom()
    {
        Debug.Log("Created A Room!");
    }
}
