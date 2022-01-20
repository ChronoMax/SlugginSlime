using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WinLevel : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void ExitLevel()
    {
        PhotonNetwork.LeaveRoom();
    }
}
