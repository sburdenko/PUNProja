using UnityEngine;
using System.Collections;
public enum PlayersSides{Cross=1,Zero=0};

public class Player {

	private PlayersSides _playerSide;
	public PlayersSides PlayerSide{
		get{
			return _playerSide;
		}
		set{
			_playerSide=value;
			if(_playerSide==PlayersSides.Cross)mark="x";
			else mark="o";
		}
	}
	public string mark;
	protected Player(){}
	private sealed class SingletonCreator
	  {
	    private static readonly Player instance = new Player();
	    public static Player Instance { get { return instance; } }
	  }
 
	  public static Player Instance
	  {
	    get { return SingletonCreator.Instance; }
	  }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
