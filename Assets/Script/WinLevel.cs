using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class WinLevel : MonoBehaviour
{
    [SerializeField]
    Text nameText;
    private void Start()
    {
        string PlayerName = PlayerPrefs.GetString("PlayerName");
        nameText.text = PlayerName;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ExitLevel()
    {
        PhotonNetwork.LeaveRoom();
    }
}
