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
        EasyAI
    }
    public Team team;
    public Type type;
    private Board board;
    private BoardState.TurnState neededTurnState;
    private bool thinking = false;
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
        if (board.boardState.turnState != neededTurnState || !board.readyForInput || thinking)
            return;

        switch (type)
        {
            case Type.Human:
                HumanInput();
                break;
            case Type.EasyAI:
                StartCoroutine(EasyAIInput());
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

    IEnumerator EasyAIInput()
    {
        thinking = true;
        yield return new WaitForSeconds(0.5f);
        var move = Random.Range(0, board.boardState.validTurns.Count);
        bool left = move < board.boardState.validTurns.Count / 2f;
        move /= 2;
        for (int m = 0; m < move; m++)
        {
            if (left)
                board.MoveLeft();
            else
                board.MoveRight();
            yield return new WaitForSeconds(0.2f);
        }
        board.MoveDown();
        thinking = false;
    }
}
