using UnityEngine;

public static class StaticData
{

   public static byte PlayersFull = 21;
   public static byte GameStart = 46;
   public static byte GreenPlayerTurn = 43;
   public static byte RedPlayerTurn = 45;
   public static byte RedPlayerIActivated = 51;
   public static byte RedPlayerIIActivated = 52;
   public static byte RedPlayerIIIActivated = 53;
   public static byte RedPlayerIVActivated = 54;
   public static byte RedPlayerIChaal = 72;
   public static byte RedPlayerIIChaal = 73;
   public static byte RedPlayerIIIChaal = 74;
   public static byte RedPlayerIVChaal = 75;
   
   public static byte GreenPlayerIActivated = 60;
   public static byte GreenPlayerIIActivated = 62;
   public static byte GreenPlayerIIIActivated = 63;
   public static byte GreenPlayerIVActivated = 64;

   public static byte GreenPlayerIChaal = 82;
   public static byte GreenPlayerIIChaal = 83;
   public static byte GreenPlayerIIIChaal = 84;
   public static byte GreenPlayerIVChaal = 85;
   
   
   public static void Log(this MonoBehaviour behav, string msg)
   {
      if (PhotonController.instance.showLogs)
      {
         Debug.Log(msg);
      }
    
   } 
   
   public static void LogColor(this MonoBehaviour behav, string msg)
   {
      if (PhotonController.instance.showLogs)
      {
         Debug.Log($"<color=yellow>{msg}</color>");
      }
   }
}
