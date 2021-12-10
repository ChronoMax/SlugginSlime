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
        view.RPC("DestroySlime", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void DestroySlime()
    {
        Destroy(gameObject);
    }
}
