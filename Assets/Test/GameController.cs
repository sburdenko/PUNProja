using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Dictionary<PlayersSides, string> SideToMark = new Dictionary<PlayersSides, string>
        {
            {PlayersSides.Cross, "x"},
            {PlayersSides.Zero, "o"}
        };

    private PlayersSides _activePlayer = PlayersSides.Cross;
    public UILabel activePlayer;
    public Transform cellPref;
    private TicTacToeCell clickedCell;
    public UILabel currPlayer;
    private TicTacToeCell[,] field;
    public Transform fieldTable;
    public UITextList log;

    private void Start()
    {
        TicTacToeCell.OnCellClick += MakeMove;
        print("start");
        CreateField();
        print("end");
        Invoke("Reposition",0.1f);
        //var bounds=NGUIMath.CalculateRelativeWidgetBounds(fieldTable);
       // fieldTable.lo

    }
    void Reposition()
    {
        fieldTable.GetComponent<UITable>().Reposition();
    }

    private void Update()
    {
        currPlayer.text = Player.Instance.PlayerSide.ToString();
        activePlayer.text = _activePlayer.ToString();
    }

    private void MakeMove(TicTacToeCell cell)
    {
        Log.Instance.AddToAll(Player.Instance.PlayerSide.ToString() + " made move");
        if (_activePlayer == Player.Instance.PlayerSide)
        {
            var pv = GetComponent<PhotonView>();
            pv.RPC("SetCell", PhotonTargets.All, cell.row, cell.col);
            if (CheckWin(cell.row, cell.col) == false)
            {
                pv.RPC("EndTurn", PhotonTargets.All);
            }
            else
            {
                Log.Instance.AddToAll(Player.Instance.PlayerSide.ToString() + "win");
            }
        }
    }

    [RPC]
    private void SetCell(int row, int col)
    {
        field[row, col].label.text = (_activePlayer == PlayersSides.Cross) ? "x" : "o";
    }

    [RPC]
    private void EndTurn()
    {
        _activePlayer = (_activePlayer == PlayersSides.Cross) ? PlayersSides.Zero : PlayersSides.Cross;
    }

     public void CreateField()
     {
         print("creating field");
         for (int i = 0; i < 3; i++)
         {
             for (int j = 0; j < 3; j++)
             {
                 var innerFieldGameObject = CreateInnerField();
                 innerFieldGameObject.transform.parent = fieldTable;
                 innerFieldGameObject.transform.localScale = Vector3.one;
                 innerFieldGameObject.GetComponent<InnerField>().row = i;
                 innerFieldGameObject.GetComponent<InnerField>().col = i;
             }
         }
         print("finished field");

     }
    public  GameObject CreateInnerField()
    {
        var innerFieldGameObject = new GameObject();
        var innerField=innerFieldGameObject.AddComponent<InnerField>();
       innerField.Cells=new TicTacToeCell[3,3];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {

                GameObject cell = NGUITools.AddChild(innerFieldGameObject, cellPref.gameObject);
                cell.GetComponent<TicTacToeCell>().row = i;
                cell.GetComponent<TicTacToeCell>().col = j;
                innerField.Cells[i,j] = cell.GetComponent<TicTacToeCell>();
            }
        }
        var table=innerFieldGameObject.AddComponent<UITable>();
        table.columns = 3;
        table.Reposition();
        return innerFieldGameObject;
    }

    private bool CheckWin(int row, int col)
    {
        int hor = 0;
        for (int i = 0; i < 3; i++)
        {
            if (field[i, col].label.text == SideToMark[_activePlayer]) hor++;
        }
        int vert = 0;
        for (int i = 0; i < 3; i++)
        {
            if (field[row, i].label.text == SideToMark[_activePlayer]) vert++;
        }

        if (hor >= 3 || vert >= 3) return true;
        return false;
    }
}