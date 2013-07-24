using UnityEngine;
using System.Collections;

public class Log : MonoBehaviour
{

    private static Log _instance;
    public static Log Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
    }

    
    public  void AddToAll(string message)
    {
      gameObject.GetComponent<PhotonView>().RPC("AddToEverybody",PhotonTargets.All,message);  
    }   

    [RPC]
    void AddToEverybody(string message)
    {
        GetComponent<UITextList>().Add(message);
    }
}
