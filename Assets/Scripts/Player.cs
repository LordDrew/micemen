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
    public enum Type
    {
        Human,
        AlwaysDownAI
    }
    public Team team;
    public Type type;
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
        if (board.boardState.turnState != neededTurnState || !board.readyForInput)
            return;

        switch (type)
        {
            case Type.Human:
                HumanInput();
                break;
            case Type.AlwaysDownAI:
                AlwaysDownAIInput();
                break;
        }
    }

    void HumanInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            board.MoveRight();
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            board.MoveLeft();
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            board.MoveUp();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            board.MoveDown();
    }

    void AlwaysDownAIInput()
    {
        board.MoveDown();
    }
}
