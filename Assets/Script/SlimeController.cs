using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlimeController : MonoBehaviour
{
    public GameObject deathText;
    public Text playerAmountText;

    public float MovementSpeed;
    public float turnSpeed;

    public GameObject slimeChunk;
    public Vector2 launchForce;

    public Vector3 cameraOffset;

    public float HitBoxScaling = 1;

    private int slime = 1;

    private Vector3 targetSize = Vector3.one;

    [HideInInspector]
    public Transform cam;
    private Vector3 targetCamPosition;
    private Vector3 targetCamRotation;

    private Vector3 oldCamPosition;
    private Vector3 oldCamRotation;

    private PhotonView View;

    private bool useFirstPersonCam = false;

    public ParticleSystem deathParticle;

    public float timeBetweenAttack;
    private float attackTime = 0;

    private Image TimeBetweenAttackVisual;
    private Text healthText;

    private void Start()
    {
        View = GetComponent<PhotonView>();

        //Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.CursorMode();

        if (View.IsMine)
        {
            cam.parent = transform;
            cam.localPosition = cameraOffset;
            oldCamPosition = cam.localPosition;
            oldCamRotation = cam.localEulerAngles;

            TimeBetweenAttackVisual = cam.GetChild(0).GetChild(0).GetComponent<Image>();
            healthText = cam.GetChild(0).GetChild(1).GetComponent<Text>();

            deathText = GameObject.Find("Canvas/DeathText");
            deathText.SetActive(false);
            playerAmountText = GameObject.Find("Canvas/PlayerAmountText").GetComponent<Text>();
        }
    }

    //[PunRPC]
    //public void TeamSetup(Vector3 color)
    //{
    //    //tag = color.ToString();
    //    GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z);
    //}


    // Update is called once per frame
    void Update()
    {
        bool tempBool = false;

        if (View.IsMine)
        {
            if (attackTime > 0)
            {
                updateAttackTimerVisual();
            }

            playerAmountText.text = "Players left: " + PhotonNetwork.CurrentRoom.PlayerCount;

            if (!tempBool && PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                tempBool = true;
                SceneManager.LoadScene("ParticleTesting");
            }

            Movement();

            CameraMovment();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                View.RPC("DecreaseSize", RpcTarget.AllBuffered, false);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && attackTime <= 0)
            {
                attackTime = timeBetweenAttack;
                TimeBetweenAttackVisual.fillAmount = 1;
                Attack();
                PlayAttackSound();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ChangeCameraModes();
            }

            if (transform.localScale != targetSize)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetSize, Time.deltaTime);
            }

            //if (Input.GetKey(KeyCode.Escape))
            //{
            //    Cursor.lockState = CursorLockMode.None;
            //}
        }
    }

    void PlayAttackSound()
    {
        gameObject.GetComponent<AudioSource>().Play();
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
        slime++;
        targetSize = slime * Vector3.one;
        UpdateHealthText();
    }

    [PunRPC]
    public void DecreaseSize(bool EnemyAttack)
    {
        if (slime - 1 > 0)
        {
            slime--;
            targetSize = slime * Vector3.one;
            UpdateHealthText();
            LaunchSlime(Random.Range(0, 360f));
        }
        else if (EnemyAttack)
        {
            View.RPC("Death", RpcTarget.AllBuffered);
            if (View.IsMine)
            {
                UpdateHealthText();
                deathText.SetActive(true);
                StartCoroutine(LeaveLobby());
            }
        }
    }

    private void LaunchSlime(float angle)
    {
        if (View.IsMine)
        {
            Transform slimePiece = PhotonNetwork.Instantiate("slimeChunk", transform.position + (transform.up * transform.localScale.y * 1.5f), Quaternion.Euler(0, angle, 0)).transform;

            Rigidbody slimeRb = slimePiece.GetComponent<Rigidbody>();
            slimeRb.AddForce(((slimePiece.forward * launchForce.x) + (slimePiece.up * launchForce.y)), ForceMode.Impulse);
            slimeRb.AddTorque(launchForce, ForceMode.Impulse);
        }
    }

    IEnumerator LeaveLobby()
    {
        yield return new WaitForSeconds(5);
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    private void Death()
    {
        MovementSpeed = 0;
        turnSpeed = 0;
        targetSize = Vector3.zero * 0.3f;
        Destroy(gameObject, 10);
    }

    private void Attack()
    {
        TargetDummyBehaviour dummy;
        SlimeController slimeScript;

        Collider[] hitCol = Physics.OverlapSphere(transform.position + (transform.forward * targetSize.z) + (transform.up * 0.35f * targetSize.y), targetSize.z / 2);
        foreach (Collider col in hitCol)
        {
            if (col.gameObject != gameObject)
            {
                if (col.TryGetComponent(out dummy))
                {
                    dummy.DecreaseSize();
                }
                else if (col.TryGetComponent(out slimeScript))
                {
                    slimeScript.GetPhotonView().RPC("DecreaseSize", RpcTarget.All, true);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("slimeChunk"))
        {
            View.RPC("IncreaseSize", RpcTarget.AllBuffered);

            View.RPC("DeleteSlimeChunk", RpcTarget.All, collision.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "DeathColl")
        {
            if (View.IsMine)
            {
                View.RPC("Death", RpcTarget.AllBuffered);
                ForcedDeath();
            }
        }
    }
    void ForcedDeath()
    {
        deathText.SetActive(true);
        StartCoroutine(LeaveLobby());
    }

    [PunRPC]
    private void DeleteSlimeChunk(int id)
    {
        PhotonView view = PhotonView.Find(id);
        if (view.IsMine)
        {
            PhotonNetwork.Destroy(view.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (View.IsMine)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + (transform.forward * targetSize.z) + (transform.up * 0.35f * targetSize.y), targetSize.z / 2 * HitBoxScaling);
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

    private void updateAttackTimerVisual()
    {
        attackTime -= Time.deltaTime;
        TimeBetweenAttackVisual.fillAmount = (attackTime / timeBetweenAttack);
    }

    private void UpdateHealthText()
    {
        healthText.text = "Health: " + slime.ToString();
    }

    public PhotonView GetPhotonView()
    {
        return View;
    }
}
