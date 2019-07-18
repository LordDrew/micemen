using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Column columnPrefab;
    public Column[] columns;
    public GameObject arrow;
    public GameObject turnIndicatorPrefab;
    public Mouse blueMousePrefab;
    public Mouse redMousePrefab;
    public GameObject blueTurnIndicator;
    public GameObject redTurnIndicator;
    int selectedTurn = 0;
    private BoardState boardState;
    private BoardState.TurnState previousTurnState = BoardState.TurnState.BlueEnd;
    private Mouse[] blueMice;
    private Mouse[] redMice;
    private GameObject[] possibleTurnsIndicators;
    // Start is called before the first frame update
    void Start()
    {
        boardState = new BoardState();
        columns = new Column[boardState.tiles.GetLength(1)];
        possibleTurnsIndicators = new GameObject[columns.Length];

        for (int i = 0; i < columns.Length; i++)
        {
            Column col = columns[i] = Instantiate(columnPrefab, transform);
            col.SetState(boardState, i);
            col.transform.localPosition = new Vector3(0.75f * i, 0);
            col.name = string.Format("col{0}", i);
            var turnIndicator = possibleTurnsIndicators[i] = Instantiate(turnIndicatorPrefab, transform);
            turnIndicator.transform.localPosition = new Vector3(0.75f * i, -0.65f);
        }
        blueMice = new Mouse[12];
        redMice = new Mouse[12];
        for (int i = 0; i < blueMice.Length; i++)
            blueMice[i] = Instantiate(blueMousePrefab, transform);
        for (int i = 0; i < redMice.Length; i++)
            redMice[i] = Instantiate(redMousePrefab, transform);
        MoveMice();
        UpdatePossibleTurns();
    }

    private void MoveMice()
    {
        for (int i = 0; i < blueMice.Length; i++)
        {
            ref Mouse mouse = ref blueMice[i];
            if (boardState.blueMicePositions[i].x < 0)
            {
                mouse.gameObject.SetActive(false);
                continue;
            }
            ref var col = ref columns[boardState.blueMicePositions[i].x];
            ref var cell = ref col.cells[boardState.blueMicePositions[i].y];
            mouse.transform.position = cell.transform.position;
            mouse.transform.SetParent(cell.transform);
        }
        for (int i = 0; i < redMice.Length; i++)
        {
            ref Mouse mouse = ref redMice[i];
            if (boardState.redMicePositions[i].x < 0)
            {
                mouse.gameObject.SetActive(false);
                continue;
            }
            ref var col = ref columns[boardState.redMicePositions[i].x];
            ref var cell = ref col.cells[boardState.redMicePositions[i].y];
            mouse.transform.position = cell.transform.position;
            mouse.transform.SetParent(cell.transform);
        }
    }

    private void UpdatePossibleTurns()
    {
        if (boardState.turnState == previousTurnState)
            return;
        previousTurnState = boardState.turnState;

        for (int i = 0; i < possibleTurnsIndicators.Length; i++)
        {
            possibleTurnsIndicators[i].SetActive(false);
        }
        if (boardState.validTurns.Count == 0)
        {
            arrow.SetActive(false);
            return;
        }
        else
        {
            arrow.SetActive(true);
        }
        for (int i = 0; i < boardState.validTurns.Count; i++)
        {
            possibleTurnsIndicators[boardState.validTurns[i]].SetActive(true);
        }
        selectedTurn = boardState.turnState == BoardState.TurnState.Blue ? boardState.validTurns.Count - 1 : 0;
        arrow.transform.localPosition = new Vector3(0.75f * boardState.validTurns[selectedTurn], -0.65f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (boardState.turnState)
        {
            case BoardState.TurnState.Blue:
            case BoardState.TurnState.BlueEnd:
                blueTurnIndicator.SetActive(true);
                redTurnIndicator.SetActive(false);
                break;
            case BoardState.TurnState.Red:
            case BoardState.TurnState.RedEnd:
                blueTurnIndicator.SetActive(false);
                redTurnIndicator.SetActive(true);
                break;
        }

        HandleInput();

        UpdatePossibleTurns();
    }

    private void HandleInput()
    {
        switch (boardState.turnState)
        {

            case BoardState.TurnState.Blue:
            case BoardState.TurnState.Red:
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    selectedTurn++;
                    selectedTurn %= boardState.validTurns.Count;

                    arrow.transform.localPosition = new Vector3(0.75f * boardState.validTurns[selectedTurn], -0.65f);
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    selectedTurn--;
                    if (selectedTurn < 0)
                        selectedTurn = boardState.validTurns.Count - 1;

                    arrow.transform.localPosition = new Vector3(0.75f * boardState.validTurns[selectedTurn], -0.65f);
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    columns[boardState.validTurns[selectedTurn]].MoveUp();
                    boardState.MoveUp(boardState.validTurns[selectedTurn]);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    columns[boardState.validTurns[selectedTurn]].MoveDown();
                    boardState.MoveDown(boardState.validTurns[selectedTurn]);
                }
                break;
            case BoardState.TurnState.BlueEnd:
            case BoardState.TurnState.RedEnd:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    boardState.MoveNext();
                    MoveMice();
                }
                break;
        }
    }
}
