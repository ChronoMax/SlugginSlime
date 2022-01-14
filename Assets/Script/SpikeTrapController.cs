using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpikeTrapController : MonoBehaviour
{
    private PhotonView photonView;
    private Animator animationController;
    private SlimeController slimeScript;

    public float damageDelay = 1;
    public float resetTime = 3;
    private float resetTimer;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        animationController = GetComponent<Animator>();
    }

    private void Update()
    {
        if (resetTimer > 0)
        {
            resetTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (resetTimer <= 0 && other.TryGetComponent<SlimeController>(out slimeScript))
        {
            photonView.RPC("TriggerTrap", RpcTarget.All);
        }
    }

    [PunRPC]
    private void TriggerTrap()
    {
        animationController.SetTrigger("TrapTrigger");
        StartCoroutine(DealDamage());
    }

    private IEnumerator DealDamage()
    {
        yield return new WaitForSeconds(damageDelay);
        resetTimer = resetTime;
        Collider[] hitCol = Physics.OverlapBox(transform.position, Vector3.one * 0.5f);
        foreach (Collider col in hitCol)
        {
            if (col.gameObject != gameObject)
            {
                if (col.TryGetComponent(out slimeScript))
                {
                    slimeScript.GetPhotonView().RPC("DecreaseSize", RpcTarget.AllBuffered, true);
                }
            }
        }
    }
}
