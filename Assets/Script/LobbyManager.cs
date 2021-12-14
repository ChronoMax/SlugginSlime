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
    [SerializeField] GameObject fpsCounter;

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

        if(fpsCounter.activeSelf)
        {
            fpsCounter.GetComponent<Text>().text = "FPS: " + (1 / Time.deltaTime).ToString("0.00");
            fpsCounter.GetComponent<Text>().text = (1f / Time.unscaledDeltaTime).ToString();
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("PhotonMainMenu");
    }

    public void ButtonExitRoom()//Button to leave the room
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ButtonFPS()//Show fps counter
    {
        fpsCounter.SetActive(!fpsCounter.activeSelf);
    }
}
