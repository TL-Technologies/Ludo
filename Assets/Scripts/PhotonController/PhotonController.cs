using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PhotonController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static PhotonController instance;
    
    [Space(5)] [Header("Connection Status")] 
    [SerializeField] private TMP_Text m_ConnectionStatus;

    [Space(10)][Header("Variables")]
    [SerializeField] internal bool showLogs;
    [SerializeField] private string roomName;
    
    [Space(10)][Header("Start Page data")]
    [SerializeField] private GameObject startPage;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button next;
    
    [Space(10)][Header("Create Room Page")]
    [SerializeField] private GameObject createRoomPage;
    [SerializeField] private Button createRoomBtn;
    [SerializeField] private Button joinRoomBtn;
    
    [Space(10)][Header("Join Room Page")]
    [SerializeField] private GameObject joinRoomPage;
    [SerializeField] private TMP_InputField roomCodeInput;
    [SerializeField] private Button joinBtn;
    
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (m_ConnectionStatus.text != null)
        {
            m_ConnectionStatus.text = "State : " + PhotonNetwork.NetworkClientState;
            
        }
      
    }


    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AddCallbackTarget(this);
        next.onClick.AddListener(OnClickNextButton);
        createRoomBtn.onClick.AddListener(CreateRoom);
        joinRoomBtn.onClick.AddListener(OnClickJoinRoomButton);
        joinBtn.onClick.AddListener(OnClickJoinBtn);
    }

    private void CreateRoom()
    {
        roomName =  Random.Range(10000, 99999).ToString();
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 });
    }


    private void OnClickNextButton()
    {
        PhotonNetwork.LocalPlayer.NickName = nameInput.text;
        startPage.SetActive(false);
        createRoomPage.SetActive(true);
        
    }

    private void OnClickJoinRoomButton()
    {
        joinRoomPage.SetActive(true);
        createRoomPage.SetActive(false);
    }

    private void OnClickJoinBtn()
    {
        string roomCode = roomCodeInput.text;
        JoinRoom(roomCode);
        
    }
    
    private void JoinRoom(string code)
    {
        PhotonNetwork.JoinRoom(code);
    }

    private void CheckMasterClient()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.LogColor("I am the Master Client");
        }
        else
        {
            this.LogColor("I am not the Master Client");
        }
    }
    #region Pun Callbacks

    public override void OnConnectedToMaster()
    {
        this.Log("Connected To Master");
        CheckMasterClient();
        next.interactable = true;
    }

    public override void OnJoinedLobby()
    {
        this.Log("Lobby joined");
    }
    
    public override void OnCreatedRoom()
    {
        this.Log(PhotonNetwork.CurrentRoom + " is created!");
        roomName = PhotonNetwork.CurrentRoom.Name;
        this.Log("Room id " + roomName);
        SceneManager.LoadScene(1);
    }

    public override void OnJoinedRoom()
    {
        this.Log(PhotonNetwork.LocalPlayer.NickName + " Joined");
        this.Log(PhotonNetwork.PlayerList.Length.ToString());

        if (PhotonNetwork.PlayerList.Length == 2)
        {
            SceneManager.LoadScene(1);
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
            this.Log("Player 1  = " + player1);
            this.Log("Player 1  = " + player2);
            RaiseEvt(StaticData.PlayersFull, data, ReceiverGroup.Others);
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        this.Log(player.NickName);
    }

    #endregion


    #region RaiseEvent


    internal void RaiseEvt(byte code, object data, ReceiverGroup options)
    {
        RaiseEventOptions receiverOptions = new RaiseEventOptions { Receivers = options };
        PhotonNetwork.RaiseEvent(code, data, receiverOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        this.Log("photon event received");

        if (photonEvent.Code == StaticData.PlayersFull)
        {
            var receivedData = (object[])photonEvent.CustomData;
            var player = (Player)receivedData[0];
            var player1Name = (string)receivedData[1];
            var player2Name = (string)receivedData[2];
            this.LogColor("Player 1 name = " + player1Name);
            this.LogColor("Player 2  name= " + player2Name);
            if (PhotonNetwork.IsMasterClient)
            {
                this.LogColor("I am the Master Client");
                GameScript.instance.startPlaying.interactable = true;
            }
        }
        
        if (photonEvent.Code == StaticData.GameStart)
        {
            GameScript.instance.StartNow();
        }
        
        if (photonEvent.Code == StaticData.GreenPlayerTurn)
        {
            Debug.Log("Got it green");
            var receivedData = (object[])photonEvent.CustomData;
            var greenTurn = (string)receivedData[1];
            this.LogColor("greenTurn " + greenTurn);
            if (!PhotonNetwork.IsMasterClient)
            {
                GameScript.instance.playerTurn = greenTurn;
                GameScript.instance.InitializeDice();
            }
        }
        
        if (photonEvent.Code == StaticData.RedPlayerTurn)
        {
            Debug.Log("Got it red");
            var receivedData = (object[])photonEvent.CustomData;
            var redTurn = (string)receivedData[1];
            this.LogColor("redTurn " + redTurn);
            if (PhotonNetwork.IsMasterClient)
            {
                GameScript.instance.playerTurn = redTurn;
                GameScript.instance.InitializeDice();
            }
        }  
    }

    #endregion
}