using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Team
    {
        Blue,
        Red,
    }
    public Team team;
    private Board board;
    private BoardState.TurnState neededTurnState;
    // Start is called before the first frame update
    void Start()
    {
        board = GetComponent<Board>();
        if (team == Team.Blue)
            neededTurnState = BoardState.TurnState.Blue;
        else
            neededTurnState = BoardState.TurnState.Red;
    }

    // Update is called once per frame
    void Update()
    {
        if (board.boardState.turnState != neededTurnState)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            board.MoveRight();
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            board.MoveLeft();
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            board.MoveUp();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            board.MoveDown();
    }
}
