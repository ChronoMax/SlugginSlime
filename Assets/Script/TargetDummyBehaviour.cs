using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummyBehaviour : MonoBehaviour
{
    public int slime;

    public GameObject slimeChunk;
    public Vector2 launchForce;


    private Vector3 targetSize = Vector3.one;

    private void Start()
    {
        targetSize = slime * Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale != targetSize)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetSize, Time.deltaTime);
        }
    }

    public void DecreaseSize()
    {
        if (slime - 1 > 0)
        {
            slime--;
            targetSize = slime * Vector3.one;
            LaunchSlime();
        }
        else
        {
            targetSize = Vector3.zero * 0.1f;
            Destroy(gameObject, 10);
        }
    }

    private void LaunchSlime()
    {
        Transform slimePiece = Instantiate(slimeChunk, transform.position + (transform.up * transform.localScale.y * 2), Quaternion.Euler(0, Random.Range(0, 360), 0)).transform;

        Rigidbody slimeRb = slimePiece.GetComponent<Rigidbody>();
        slimeRb.AddForce(((slimePiece.forward * launchForce.x) + (slimePiece.up * launchForce.y)), ForceMode.Impulse);
        slimeRb.AddTorque(launchForce, ForceMode.Impulse);
    }
}
