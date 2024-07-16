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
    [Header("Game UI And Game Logic")]
    [SerializeField] GameUI gameUI;
    [SerializeField] GameLogic gameLogic;


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

    internal void RedlayerDice(int diceId,int diceCount)
    {
        object[] data =
        {
          PhotonNetwork.LocalPlayer,
          diceId,
          diceCount
        };
        RaiseEvt(StaticData.RED_DICE, data, ReceiverGroup.Others);
    } 
    
    internal void GreenlayerDice(int diceId, int diceCount)
    {
        object[] data =
        {
          PhotonNetwork.LocalPlayer,
          diceId,
          diceCount
        };
        RaiseEvt(StaticData.GREEN_DICE, data, ReceiverGroup.Others);
    }

    internal void MoveRed(int playerID,int pieceID)
    {
        object[] data =
        {
          playerID,
          pieceID
        };
        RaiseEvt(StaticData.MOVE_RED, data, ReceiverGroup.Others);
    } 
    
    internal void MoveGreen(int playerID,int pieceID)
    {
        object[] data =
        {
          playerID,
          pieceID
        };
        RaiseEvt(StaticData.MOVE_GREEN, data, ReceiverGroup.Others);
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


            case StaticData.RED_DICE:
                object[] redDiceData = (object[])photonEvent.CustomData;
                int redDiceId = (int)redDiceData[1];
                int redDiceCount = (int)redDiceData[2];
                Debug.Log("Red Dice Id Received --> " + redDiceId);
                gameLogic.GenerateDiceCountForOther(redDiceId + 1, redDiceCount);
                break;
                    
            case StaticData.GREEN_DICE:
                object[] greenDiceData = (object[])photonEvent.CustomData;
                int greenDiceId = (int)greenDiceData[1];
                int greenDiceCount = (int)greenDiceData[2];
                Debug.Log("Green Dice Id Received --> " + greenDiceId);
                gameLogic.GenerateDiceCountForOther(greenDiceId - 1, greenDiceCount);
                break;
            
            case StaticData.MOVE_RED:
                object[] moveRedData = (object[])photonEvent.CustomData;
                int redPlayerId = (int)moveRedData[0];
                int redPieceId = (int)moveRedData[1];
                Debug.Log($"Moving data Red --> PlayerId --> {redPlayerId} ---> PieceId --> {redPieceId} "  );
                gameLogic.MovePiece(redPlayerId + 1 , redPieceId);
                break;

            case StaticData.MOVE_GREEN:
                object[] moveGreenData = (object[])photonEvent.CustomData;
                int greenPlayerId = (int)moveGreenData[0];
                int greenPieceId = (int)moveGreenData[1];
                Debug.Log($"Moving data green --> PlayerId --> {greenPlayerId} ---> PieceId --> {greenPieceId} "  );
                gameLogic.MovePiece(greenPlayerId - 1 , greenPieceId);
                break;

            default: 
                break;
        }


    }

    #endregion

}
