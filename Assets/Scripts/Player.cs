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
        EasyAI,
        MediumAI,
        HardAI
    }
    public Team team;
    public Type type;
    private Board board;
    private BoardState.TurnState activeTurnState;
    private BoardState.TurnState targetTurnState;
    private bool thinking = false;
    // Start is called before the first frame update
    void Start()
    {
        int playerType;
        board = GetComponent<Board>();
        if (team == Team.Blue)
        {
            activeTurnState = BoardState.TurnState.Blue;
            targetTurnState = BoardState.TurnState.BlueVictory;
            playerType = PlayerPrefs.GetInt(Settings.bluePlayerKey, 0);
        }
        else
        {
            activeTurnState = BoardState.TurnState.Red;
            targetTurnState = BoardState.TurnState.RedVictory;
            playerType = PlayerPrefs.GetInt(Settings.redPlayerKey, 1);
        }
        switch (playerType)
        {
            case 0:
                type = Type.Human;
                break;
            case 1:
                type = Type.EasyAI;
                break;
            case 2:
                type = Type.MediumAI;
                break;
            case 3:
                type = Type.HardAI;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (board.boardState.turnState != activeTurnState || !board.readyForInput || thinking)
            return;

        switch (type)
        {
            case Type.Human:
                HumanInput();
                break;
            case Type.EasyAI:
                StartCoroutine(EasyAIInput());
                break;
            case Type.MediumAI:
                StartCoroutine(MCTSAIInput(10));
                break;
            case Type.HardAI:
                StartCoroutine(MCTSAIInput(30));
                break;
        }
    }

    Vector2 mouseDownPosition = Vector2.zero;
    float waitTime;
    bool movedHorizontally;
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

        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPosition = Input.mousePosition;
            movedHorizontally = false;
            waitTime = 0;
        }
        if (Input.GetMouseButton(0))
        {
            var delta = (Vector2)Input.mousePosition - mouseDownPosition;
            if (delta.magnitude > 100)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y) * 2)
                {
                    if (waitTime > 0)
                        waitTime -= Time.deltaTime;
                    else
                    {
                        if (delta.x < 0)
                        {
                            movedHorizontally = true;
                            waitTime = 0.3f;
                            board.MoveLeft();
                        }
                        else
                        {
                            movedHorizontally = true;
                            waitTime = 0.3f;
                            board.MoveRight();
                        }
                    }
                }
                else
                    waitTime = 0;
            }
            else
                waitTime = 0;
        }
        if (Input.GetMouseButtonUp(0) && !movedHorizontally)
        {
            var delta = (Vector2)Input.mousePosition - mouseDownPosition;
            Debug.Log(delta);
            if (delta.magnitude > 100)
            {
                if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x) * 2)
                {
                    if (delta.y < 0)
                        board.MoveDown();
                    else
                        board.MoveUp();
                }
            }
        }
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
        for (int m = 0; m < board.boardState.validTurns.Count; m++)
        {
            var score = TryMove(board.boardState.validTurns[m], false);
            if (score < bestScore)
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
    IEnumerator MCTSAIInput(int budget)
    {
        thinking = true;

        MCTS search = new MCTS(board.boardState, targetTurnState);
        for (int i = 0; i < budget; i++)
        {
            for (int j = 0; j < 100; j++)
                search.Iterate();
            yield return null;
        }
        MCTS.Move bestMove = search.GetBestMove();
        while (board.selectedTurn != bestMove.MoveIndex)
        {
            if (team == Team.Blue)
                board.MoveLeft();
            else
                board.MoveRight();
            yield return new WaitForSeconds(0.1f);
        }

        if (bestMove.Up)
            board.MoveUp();
        else
            board.MoveDown();
        thinking = false;
    }
}
