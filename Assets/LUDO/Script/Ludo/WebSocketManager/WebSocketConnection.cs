using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using TMPro;

public class WebSocketConnection : MonoBehaviour
{
    WebSocket websocket;
    public static WebSocketConnection instance;

    [SerializeField] internal long myId;
    [SerializeField] internal TMP_Text idText;
    
    [Serializable]
    public class sampleData
    {
        public string roomId;
        public string user1;
        public string user2;
    }

    private void Awake()
    {
        instance = this;    
    }

    public sampleData data = new sampleData();  

    async void Start()
    {
        GeneraterandomId();
        // websocket = new WebSocket("ws://echo.websocket.org");
        websocket = new WebSocket("ws://localhost:3000");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received Message -->  " + message);
            data = JsonUtility.FromJson<sampleData>(message);
            if (data.user1 == myId.ToString() || data.user2 == myId.ToString())
            {
                PhotonController.instance.CreateRoomExt(data.roomId);
            }
            
        };

       
    }

    public async void sdh()
    {
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    [ContextMenu("Send to start")]
   public async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("Please Start");
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

   
    public void GeneraterandomId()
    {
        myId = UnityEngine.Random.Range(1000000,9999999);
        idText.text  = myId.ToString();
    }
}
