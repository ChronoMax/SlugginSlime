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
    [SerializeField] Text textButton;

        [Header("Canvasses")]
    [SerializeField] GameObject mainCV;
    [SerializeField] GameObject htpCV;

    private List<RoomInfo> roomList;

    private bool foundRoom = false;
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        if (!PhotonNetwork.IsConnected)
        {
            debugText.text = "Connecting to servers...";
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.ConnectToRegion("eu");

            playButton.interactable = false;
            joinButton.interactable = false;
        }
    }

    public void Play()
    {
        if (PhotonNetwork.IsConnected)
        {
            StartCoroutine(JoinRandomRoom());
        }
    }

    public void SearchGame()
    {
        if (PhotonNetwork.IsConnected)
        {
            StartCoroutine(CreateRandomRoom());
        }
    }

    public void UpdateRoomList()
    {
        if (roomName.text.Length == 5)
        {
            foundRoom = CheckRoom();
            if (CheckRoom())
            {
                textButton.text = "Join";
            }
            else
            {
                textButton.text = "Room not found";
            }
        }
        else
        {
            textButton.text = "Create";
        }

        PhotonNetwork.JoinLobby();
    }
    IEnumerator JoinRandomRoom()
    {
        yield return new WaitForSeconds(1);

        PhotonNetwork.JoinRandomRoom();
    }

    IEnumerator CreateRandomRoom()
    {
        yield return new WaitForSecondsRealtime(3);
        string randomRoom = Random.Range(1, 100000).ToString();

        RoomOptions roomOpsSpecial = new RoomOptions()
        {
            IsVisible = true, // Private game?
            IsOpen = true, // Joinable?
            MaxPlayers = (byte)4, // RoomSize in Bytes
        };

        if (roomName.text.Length == 5 && foundRoom == true)
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

    private bool CheckRoom()
    {
        if (roomList != null)
        {
            foreach (RoomInfo room in roomList)
            {
                if (room.Name == roomName.text)
                {
                    return true;
                }
            }
        }
        return false;
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
        joinButton.interactable = true;
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

    public override void OnRoomListUpdate(List<RoomInfo> p_list)
    {
        roomList = p_list;
        base.OnRoomListUpdate(roomList);
    }

    public void HowToPlayPressed()
    {
        mainCV.SetActive(false);
        htpCV.SetActive(true);
    }

    public void BackBtn()
    {
        htpCV.SetActive(false);
        mainCV.SetActive(true);
    }

    public void ExitGameBtn()
    {
        Application.Quit();
    }
}