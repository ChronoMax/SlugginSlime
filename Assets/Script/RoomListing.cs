using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class RoomListing : MonoBehaviourPunCallbacks
{
    public List<RoomInfo> roomList;

    public void clickRefresh()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> p_list)
    {
        roomList = p_list;
        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room.Name);
        }

        base.OnRoomListUpdate(roomList);
    }
}
