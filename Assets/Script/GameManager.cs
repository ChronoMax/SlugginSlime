using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Text RoomText;
    [SerializeField] GameObject exitPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject fpsCounter;

    private bool toggle;
    private float timer;
    private float hudRefreshRate = 1f;

    #region Singleton
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    #endregion

    void Start()
    {
        RoomText.text = PhotonNetwork.CurrentRoom.Name;
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            CursorMode();
            exitPanel.SetActive(!exitPanel.activeSelf);
            if (settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(!settingsPanel.activeSelf);
                exitPanel.SetActive(false);
            }
        }

        if (fpsCounter.activeSelf)
        {
            if (Time.unscaledTime > timer)
            {
                int fps = (int)(1f / Time.unscaledDeltaTime);
                fpsCounter.GetComponent<Text>().text = fps.ToString();
                timer = Time.unscaledTime + hudRefreshRate;
            }
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("PhotonMainMenu");
        Destroy(gameObject);
    }

    public void ButtonExitRoom()//Button to leave the room
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ButtonFPS(bool toggle)//Show fps counter
    {
        fpsCounter.SetActive(toggle);
    }

    public void ButtonResume()
    {
        CursorMode();
    }

    public void CursorMode()
    {
        toggle = !toggle;
        if(SceneManager.GetActiveScene().name != "LobbyMax")
        {
            if (toggle)
            {
                Cursor.lockState = CursorLockMode.None;
                Debug.Log("Cursor lockstate: Unlocked");

            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Debug.Log("Cursor lockstate: Locked");
            }
        }
    }
}
