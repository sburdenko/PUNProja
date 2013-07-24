using UnityEngine;
using System.Collections;
using System;

public class TicTacToeCell : MonoBehaviour {
	public UITextList log;
	public static event Action<TicTacToeCell> OnCellClick;
	public UILabel label;
	public int row,col;
	
	void OnClick(){
		OnCellClick(gameObject.GetComponent<TicTacToeCell>());
	}
	
}
