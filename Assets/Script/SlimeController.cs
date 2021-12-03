using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SlimeController : MonoBehaviour
{
    public float MovementSpeed;
    public float turnSpeed;

    public GameObject slimeChunk;

    private int slime = 1;

    private Vector3 targetSize = Vector3.one;

    private PhotonView View;

    private void Start()
    {
        View = GetComponent<PhotonView>();

        Cursor.lockState = CursorLockMode.Locked;

        if (View.IsMine)
        {
            Camera.main.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        CameraMovment();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DecreaseSize();
        }


        if (transform.localScale != targetSize)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetSize, Time.deltaTime);
        }
    }

    public void Movement()
    {
        Vector3 moveVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (moveVector != Vector3.zero)
        {
            transform.Translate(moveVector * MovementSpeed / (float)slime * Time.deltaTime);
        }
    }

    public void CameraMovment()
    {
        float mouseInputY = Input.GetAxis("Mouse X") * turnSpeed;

        if (mouseInputY != 0)
        {
            mouseInputY += transform.eulerAngles.y;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, mouseInputY, transform.eulerAngles.z);
        }
    }

    private void IncreaseSize()
    {
        slime++;
        targetSize = slime * Vector3.one;
    }

    private void DecreaseSize()
    {
        if (slime - 1 > 0)
        {
            slime--;
            targetSize = slime * Vector3.one;
            Instantiate(slimeChunk, new Vector3(transform.position.x, 1, transform.position.z) + (transform.forward * 5), Quaternion.identity);
        }
        else
        {
            //TODO: deathFlag
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("slimeChunk"))
        {
            IncreaseSize();
            Destroy(collision.gameObject);
        }
    }
}
