using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SlimeController : MonoBehaviour
{
    public float MovementSpeed;
    public float turnSpeed;

    public GameObject slimeChunk;
    public Vector2 launchForce;

    public Vector3 cameraOffset;

    public float HitBoxScaling = 1;

    private int slime = 1;

    private Vector3 targetSize = Vector3.one;

    Transform cam;
    private Vector3 targetCamPosition;
    private Vector3 targetCamRotation;

    private Vector3 oldCamPosition;
    private Vector3 oldCamRotation;

    private PhotonView View;


    private bool useFirstPersonCam = false;


    private void Start()
    {
        View = GetComponent<PhotonView>();

        Cursor.lockState = CursorLockMode.Locked;

        if (View.IsMine)
        {
            cam = Camera.main.transform;
            cam.parent = transform;
            oldCamPosition = cam.localPosition;
            oldCamRotation = cam.localEulerAngles;
        }

        View.RPC("TeamSetup", RpcTarget.AllBuffered, new Vector3(Color.red.r, Color.red.b, Color.red.g));
    }

    [PunRPC]
    public void TeamSetup(Vector3 color)
    {
        //tag = color.ToString();
        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z);
    }


    // Update is called once per frame
    void Update()
    {
        if (View.IsMine)
        {
            Movement();

            CameraMovment();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                View.RPC("DecreaseSize", RpcTarget.AllBuffered, false);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Attack();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ChangeCameraModes();
            }

            if (transform.localScale != targetSize)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetSize, Time.deltaTime);
            }
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

    public void ChangeCameraModes()
    {
        useFirstPersonCam = !useFirstPersonCam;
        if (useFirstPersonCam)
        {
            targetCamPosition = Vector3.zero;
            targetCamRotation = Vector3.zero;
        }
        else
        {
            targetCamPosition = oldCamPosition;
            targetCamRotation = oldCamRotation;
        }

        StopAllCoroutines();
        StartCoroutine(MoveAndRotateCamera());
    }

    [PunRPC]
    private void IncreaseSize()
    {
        if (View.IsMine)
        {
            slime++;
            targetSize = slime * Vector3.one;
        }
    }

    [PunRPC]
    private void DecreaseSize(bool EnemyAttack)
    {
        if (slime - 1 > 0)
        {
            slime--;
            targetSize = slime * Vector3.one;
            LaunchSlime();
        }
        else if (EnemyAttack)
        {
            View.RPC("Death", RpcTarget.AllBuffered);
        }
    }

    private void LaunchSlime()
    {
        Transform slimePiece = Instantiate(slimeChunk, transform.position + (transform.up * transform.localScale.y), Quaternion.Euler(0, Random.Range(0, 360), 0)).transform;

        Rigidbody slimeRb = slimePiece.GetComponent<Rigidbody>();
        slimeRb.AddForce(((slimePiece.forward * launchForce.x) + (slimePiece.up * launchForce.y)), ForceMode.Impulse);
        slimeRb.AddTorque(launchForce, ForceMode.Impulse);
    }

    [PunRPC]
    private void Death()
    {
        MovementSpeed = 0;
        turnSpeed = 0;
        targetSize = Vector3.zero * 0.1f;
        Destroy(gameObject, 10);
    }

    private void Attack()
    {
        TargetDummyBehaviour dummy;
        SlimeController slimeScript;

        Collider[] hitCol = Physics.OverlapSphere(transform.position + (transform.forward * targetSize.z), targetSize.z / 2);
        foreach (Collider col in hitCol)
        {
            //if (col.TryGetComponent(out photonviewer) && !photonviewer.IsMine)
            //{
            if (col.gameObject != gameObject)
            {
                if (col.TryGetComponent(out dummy))
                {
                    dummy.DecreaseSize();
                }
                else if (col.TryGetComponent(out slimeScript))
                {
                    slimeScript.GetPhotonView().RPC("DecreaseSize", RpcTarget.AllBuffered, true);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("slimeChunk"))
        {
            View.RPC("IncreaseSize", RpcTarget.AllBuffered);
            View.RPC("CollectSlime", RpcTarget.AllBuffered, collision.gameObject);
        }
    }

    [PunRPC]
    private void CollectSlime(GameObject slimeChunk)
    {
        Destroy(slimeChunk);
    }

    private void OnDrawGizmos()
    {
        if (View.IsMine)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + (transform.forward * targetSize.z), targetSize.z / 2 * HitBoxScaling);
        }
    }

    private IEnumerator MoveAndRotateCamera()
    {
        while (Vector3.Distance(cam.position, targetCamPosition) >= 0.5f && cam.eulerAngles != targetCamRotation)
        {
            cam.localPosition = Vector3.Lerp(cam.localPosition, targetCamPosition, Time.deltaTime * 10);
            cam.localEulerAngles = Vector3.Lerp(cam.localEulerAngles, targetCamRotation, Time.deltaTime * 10);
            yield return new WaitForSeconds(0.000001f);
        }
    }

    public PhotonView GetPhotonView()
    {
        return View;
    }
}
