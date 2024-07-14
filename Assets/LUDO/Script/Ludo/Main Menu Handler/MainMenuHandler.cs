using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public static MainMenuHandler instance;


    [Space(10)]
    [Header("Panels")]
    [SerializeField] internal GameObject namePanel;
    [SerializeField] internal GameObject mainMenu;
    [SerializeField] internal GameObject createJoinRoomPanel;
    [SerializeField] internal GameObject JoinRoomWithCodePanel;

    [Space(10)]
    [Header("Input and  Button")]
    [SerializeField] internal InputField _name;
    [SerializeField] internal InputField roomid;
    [SerializeField] internal Button _nextButton;
    [SerializeField] internal Button createRoomButton;
    [SerializeField] internal Button joinRoomWithCodeButton;
    [SerializeField] internal Button joinRoomButton;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _nextButton.interactable = false;
        _nextButton.onClick.AddListener(OnNameSubmit);
        createRoomButton.onClick.AddListener(OnClickCreateRoom);
        joinRoomWithCodeButton.onClick.AddListener(OpenJoinWithCode);
        joinRoomButton.onClick.AddListener(JoinRoom);    

    }


    public void OnNameSubmit()
    {
        if (string.IsNullOrEmpty(_name.text) || string.IsNullOrWhiteSpace(_name.text))
        {
            return;

        }
        StaticData.SetPlayerName(_name.text);
        PhotonNetwork.LocalPlayer.NickName = StaticData.GetPlayerName();
        OpenCreateJoinWindow();
    }



    private void OpenCreateJoinWindow()
    {
        namePanel.SetActive(false);
        createJoinRoomPanel.SetActive(true);
    }

    private void OpenJoinWithCode()
    {
        createJoinRoomPanel.SetActive(false);
        JoinRoomWithCodePanel.SetActive(true);

    }

    private void OnClickCreateRoom()
    {
        PhotonController.instance.CreateRoom();
        createJoinRoomPanel.SetActive(false);
        mainMenu.SetActive(true);

    }

    private void JoinRoom()
    {
        if (string.IsNullOrEmpty(roomid.text) || string.IsNullOrWhiteSpace(roomid.text) || roomid.text.Length <5)
        {
            return;
        }
        Debug.Log("Hello");
        PhotonController.instance.JoinRoom(roomid.text);
        JoinRoomWithCodePanel.SetActive(false);
        mainMenu.SetActive(true);
    }

    internal void SetNextButtonInteractable(bool state)
    {
        _nextButton.interactable = state;
    }
}
