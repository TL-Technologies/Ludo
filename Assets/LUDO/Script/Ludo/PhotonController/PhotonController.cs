using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static PhotonController instance;

    [Space(5)]
    [Header("Connection Status")]
    [SerializeField] private TMP_Text _ConnectionStatus;
    [SerializeField] private TMP_Text waitMessage;

    [Space(5)]
    [Header("Room id")]
    [SerializeField] internal string roomId;



    #region unity events
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void Update()
    {
        if (_ConnectionStatus.text != null)
        {
            _ConnectionStatus.text = "State : " + PhotonNetwork.NetworkClientState;

            if (PhotonNetwork.NetworkClientState == ClientState.Joined && roomId != null)
            {
                _ConnectionStatus.text = "State : " + PhotonNetwork.NetworkClientState +  " In " + roomId;
            }

        }
    }

    #endregion


    #region internal methods


    internal void CreateRoom()
    {
        string roomName = Random.Range(10000, 99999).ToString();
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 });
    }

    internal void JoinRoom(string code)
    {
        PhotonNetwork.JoinRoom(code);
    }

    internal void DisableBottomMessage()
    {
        waitMessage.text = " ";
    }

    #endregion


    #region photon events

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");
        MainMenuHandler.instance.SetNextButtonInteractable(true);
        DisableBottomMessage();
    }

    public override void OnCreatedRoom()
    {
        roomId = PhotonNetwork.CurrentRoom.Name;
        Debug.Log( "Room --> "+  roomId + " is created!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Joined the room");
        Debug.Log("Total Players -->" + PhotonNetwork.PlayerList.Length.ToString());
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    public void OnEvent(EventData photonEvent)
    {
    }

    #endregion

}
