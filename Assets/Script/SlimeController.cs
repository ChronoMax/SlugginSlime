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

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
    }

    public void Movement()
    {
        Vector3 moveVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (moveVector != Vector3.zero)
        {
            transform.Translate(moveVector * MovementSpeed * Time.deltaTime);
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
        transform.localScale = slime * Vector3.one;
    }

    private void DecreaseSize()
    {
        if (slime - 1 > 0)
        {
            slime--;
            transform.localScale = slime * Vector3.one;
            Instantiate(slimeChunk, transform.position + (transform.forward * 10), Quaternion.identity);
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
