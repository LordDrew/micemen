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

    int TryMove(int move, bool up)
    {
        BoardState bs = new BoardState(board.boardState);
        if (up)
            bs.MoveUp(move);
        else
            bs.MoveDown(move);
        while (bs.MoveNext()) ;

        if (team == Team.Blue)
        {
            int fieldWidth = bs.tiles.GetLength(1);
            int blueDistance = 0;
            foreach (var mouse in bs.blueMice)
            {
                if (mouse.IsActive)
                    blueDistance += (fieldWidth - mouse.X) * (fieldWidth - mouse.X);
            }
            return blueDistance;
        }
        else
        {
            int redDistance = 0;
            foreach (var mouse in bs.redMice)
            {
                if (mouse.IsActive)
                    redDistance += mouse.X * mouse.X;
            }
            return redDistance;
        }
    }

    IEnumerator EasyAIInput()
    {
        thinking = true;
        yield return new WaitForSeconds(0.5f);

        int bestScore = int.MaxValue;
        int bestMoveIndex = 0;
        bool bestMoveIsUp = false;
        for(int m = 0; m < board.boardState.validTurns.Count; m++)
        {
            var score = TryMove(board.boardState.validTurns[m], false);
            if(score < bestScore)
            {
                bestScore = score;
                bestMoveIsUp = false;
                bestMoveIndex = m;
            }
            score = TryMove(board.boardState.validTurns[m], true);
            if (score < bestScore)
            {
                bestScore = score;
                bestMoveIsUp = true;
                bestMoveIndex = m;
            }
        }
        while (board.selectedTurn != bestMoveIndex)
        {
            if (team == Team.Blue)
                board.MoveLeft();
            else
                board.MoveRight();
            yield return new WaitForSeconds(0.3f);
        }

        if (bestMoveIsUp)
            board.MoveUp();
        else
            board.MoveDown();
        thinking = false;
    }
}
