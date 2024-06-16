using UnityEngine;

public static class StaticData
{

   public static byte PlayersFull = 21;
   public static byte GameStart = 46;
   public static byte GreenPlayerTurn = 43;
   public static byte RedPlayerTurn = 45;
   
   
   
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
