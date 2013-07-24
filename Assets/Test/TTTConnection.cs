using UnityEngine;
using System.Collections;

public class TTTConnection : MonoBehaviour {

	public UITextList log;
	public Transform panel;
	public PhotonView hex;
	void Start () {
		log.Add("start");
		PhotonNetwork.ConnectUsingSettings("1.0");
		//Invoke("IsMine",10);
	}
	
	
	void Update () {
	 
	}
	public virtual void OnConnectedToPhoton(){
		log.Add("connection success");
		//PhotonNetwork.JoinRandomRoom();
	}
	public virtual void OnJoinedLobby(){
		log.Add("lobby joined");
		PhotonNetwork.JoinRandomRoom();
		log.Add("joining random");
		
	}
	public virtual void OnPhotonRandomJoinFailed(){
		log.Add("join random failed");
		log.Add("CreaingRoom");
		PhotonNetwork.CreateRoom(null, true, true, 2);
	}
	public virtual void OnCreatedRoom(){
		/*var hexGO=PhotonNetwork.Instantiate("hex",Vector3.zero,Quaternion.identity,0,null);
		hexGO.transform.parent=panel;
		hexGO.transform.localPosition=Vector3.zero;
		hexGO.transform.localScale=new Vector3(50f,50f,0);
		hex=hexGO.GetComponent<PhotonView>();*/
		log.Add("Room created");
	}
	public virtual void OnJoinedRoom(){
		log.Add("Room joined");
		if(PhotonNetwork.isMasterClient)Player.Instance.PlayerSide=PlayersSides.Cross;
		else Player.Instance.PlayerSide=PlayersSides.Zero;
		
		
		
	}
	public virtual void OnFailedToConnectToPhoton()
	{
		log.Add("connection failed");
	}
	void IsMine(){
		log.Add (hex.isMine.ToString()+hex.owner.ToString());
		//hex.owner=PhotonNetwork.Pl
	}
}
