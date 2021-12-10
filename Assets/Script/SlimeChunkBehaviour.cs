using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SlimeChunkBehaviour : MonoBehaviour
{
    private PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public void CollectSlime()
    {
        view.RPC("DestroySlime", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void DestroySlime()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
