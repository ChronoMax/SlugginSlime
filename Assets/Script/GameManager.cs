using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviourPunCallbacks, IPointerEnterHandler, IPointerExitHandler
{
    public Text RoomText;
    [SerializeField] GameObject toolTip;
    [SerializeField] GameObject exitPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject fpsCounter;

    public InputField nameInputField;

    private string roomName;
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
        nameInputField.characterLimit = 10;

        roomName = string.Format("Room ID: {0}", PhotonNetwork.CurrentRoom.Name);
        RoomText.text = roomName;

        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            return;
        }
        else
        {
            string PlayerName = PlayerPrefs.GetString("PlayerName");
            nameInputField.text = PlayerName;
        }
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
                fpsCounter.GetComponent<Text>().text = "FPS: " + fps.ToString();
                timer = Time.unscaledTime + hudRefreshRate;
            }
        }

        if (SceneManager.GetActiveScene().name != "LobbyMax_4" && SceneManager.GetActiveScene().name != "LobbyMax_20")
        {
            toolTip.SetActive(false);
            RoomText.text = "";
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

    public void CopyRoomID()
    {
        StartCoroutine(CopyToClipboard());
    }
    private IEnumerator CopyToClipboard()
    {
        TextEditor textEditor = new TextEditor();

        if(PhotonNetwork.CurrentRoom.Name != null)
        {
            textEditor.text = PhotonNetwork.CurrentRoom.Name;
            textEditor.SelectAll();
            textEditor.Copy(); //Copy room name to clipboard

            RoomText.text = "Copied";

            yield return new WaitForSeconds(1f);

            RoomText.text = roomName;
        }

        yield return null;
    }

    public void CursorMode()
    {
        toggle = !toggle;
        if(SceneManager.GetActiveScene().name != "LobbyMax_4" && SceneManager.GetActiveScene().name != "LobbyMax_20")
        {
            if (toggle)
            {
                Cursor.lockState = CursorLockMode.None;

            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if(eventData.pointerEnter.name == "RoomName")
        {
            toolTip.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(eventData.pointerEnter.name == "RoomName")
        {
            toolTip.SetActive(false);
        }
    }

    public void PlacePLayerName()
    {
        string playerNickname = nameInputField.text;
        PhotonNetwork.NickName = playerNickname;
        PlayerPrefs.SetString("PlayerName", playerNickname);
    }

    public InputField GetInput()
    {
        return nameInputField;
    }

    public bool GetSettingsPanel()
    {
        if(exitPanel.activeSelf || settingsPanel.activeSelf)
        {
            return true;
        }
        return false;
    }
}
