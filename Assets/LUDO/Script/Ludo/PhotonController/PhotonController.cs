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
    [Header("Game UI")]
    [SerializeField] GameUI gameUI;


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

    internal void RaiseEvt(byte code, object data, ReceiverGroup options)
    {
        RaiseEventOptions receiverOptions = new RaiseEventOptions { Receivers = options };
        PhotonNetwork.RaiseEvent(code, data, receiverOptions, SendOptions.SendReliable);
    }

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
        roomId = PhotonNetwork.CurrentRoom.Name;
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Joined the room");
        Debug.Log("Total Players -->" + PhotonNetwork.PlayerList.Length.ToString());


        if (PhotonNetwork.PlayerList.Length == 2)
        {
            gameUI.ShowScreen(gameUI.gameplayBoard);
            gameUI.ShowScreen(MainMenuHandler.instance.waitingPanel);
            gameUI.HideScreen(MainMenuHandler.instance.JoinRoomWithCodePanel);
            gameUI.HideScreen(MainMenuHandler.instance.mainMenu);
            gameUI.HideScreen(gameUI.gameOverScreenPanel);
            gameUI.StopMainScreenAnimations();
            gameUI.DisableAllWinElements();
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            string player1 = PhotonNetwork.PlayerList[0].NickName;
            string player2 = PhotonNetwork.PlayerList[1].NickName;
            object[] data =
            {
                PhotonNetwork.LocalPlayer,
                player1,
                player2
            };
            RaiseEvt(StaticData.PLAYER_JOINED, data, ReceiverGroup.Others);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    public void OnEvent(EventData photonEvent)
    {
        var code = photonEvent.Code;

        switch (code)
        {

            case StaticData.PLAYER_JOINED:
                object[] data = (object[])photonEvent.CustomData;
                var  player = (Player)data[0];
                string player1 = (string)data[1];
                string player2 = (string)data[2];
                if (PhotonNetwork.IsMasterClient)
                {
                    MainMenuHandler.instance.startGameButton.interactable = true;
                }
                break;

            case StaticData.GAME_START:
                gameUI.OnClickPlay();
                break;

            default: 
                break;
        }


    }

    #endregion

}
