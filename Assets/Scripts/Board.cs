using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public Column columnPrefab;
    public Column[] columns;
    public GameObject arrow;
    public GameObject turnIndicatorPrefab;
    public GameObject ban;
    public Mouse blueMousePrefab;
    public Mouse redMousePrefab;
    public GameObject blueTurnIndicator;
    public GameObject redTurnIndicator;
    public Text blueScore;
    public Text redScore;
    public GameObject blueVictory;
    public GameObject redVictory;
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
        StartCoroutine(MoveMice(0f));
        UpdatePossibleTurns();
    }

    private IEnumerator MoveMice(float time)
    {
        for (int i = 0; i < blueMice.Length; i++)
        {
            if (!boardState.blueMice[i].IsActive)
            {
                blueMice[i].gameObject.SetActive(false);
                continue;
            }
            Cell cell = columns[boardState.blueMice[i].X]
                .cells[boardState.blueMice[i].Y];
            if (blueMice[i].transform.parent != cell.transform)
            {
                if (time > 0)
                    yield return blueMice[i].MoveToCell(cell, time);
                else
                    StartCoroutine(blueMice[i].MoveToCell(cell, 0f));
            }
        }
        for (int i = 0; i < redMice.Length; i++)
        {
            if (!boardState.redMice[i].IsActive)
            {
                redMice[i].gameObject.SetActive(false);
                continue;
            }
            Cell cell = columns[boardState.redMice[i].X]
                .cells[boardState.redMice[i].Y];
            if (redMice[i].transform.parent != cell.transform)
            {
                if (time > 0)
                    yield return redMice[i].MoveToCell(cell, time);
                else
                    StartCoroutine(redMice[i].MoveToCell(cell, 0f));
            }
        }
    }

    IEnumerator AutoMoveMice()
    {
        yield return new WaitForSeconds(0.75f);
        while (boardState.MoveNext())
        {
            yield return StartCoroutine(MoveMice(.5f));
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
            ban.SetActive(false);
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
        if (boardState.moveBanned)
        {
            ban.SetActive(true);
            possibleTurnsIndicators[boardState.lastTurn].SetActive(true);
            ban.transform.position = possibleTurnsIndicators[boardState.lastTurn].transform.position;
        }
        else
        {
            ban.SetActive(false);
        }
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
            case BoardState.TurnState.BlueVictory:
                blueVictory.SetActive(true);
                break;
            case BoardState.TurnState.RedVictory:
                redVictory.SetActive(true);
                break;
        }

        UpdatePossibleTurns();
        HandleInput();

        blueScore.text = boardState.blueScore.ToString();
        redScore.text = boardState.redScore.ToString();
    }

    public void MoveLeft()
    {
        selectedTurn--;
        if (selectedTurn < 0)
            selectedTurn = boardState.validTurns.Count - 1;

        arrow.transform.localPosition = new Vector3(0.75f * boardState.validTurns[selectedTurn], -0.65f);
    }
    public void MoveRight()
    {
        selectedTurn++;
        selectedTurn %= boardState.validTurns.Count;

        arrow.transform.localPosition = new Vector3(0.75f * boardState.validTurns[selectedTurn], -0.65f);
    }
    public void MoveUp()
    {
        columns[boardState.validTurns[selectedTurn]].MoveUp();
        boardState.MoveUp(boardState.validTurns[selectedTurn]);
        StartCoroutine(AutoMoveMice());
    }
    public void MoveDown()
    {
        columns[boardState.validTurns[selectedTurn]].MoveDown();
        boardState.MoveDown(boardState.validTurns[selectedTurn]);
        StartCoroutine(AutoMoveMice());
    }
    private void HandleInput()
    {
        switch (boardState.turnState)
        {

            case BoardState.TurnState.Blue:
            case BoardState.TurnState.Red:
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    MoveRight();
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    MoveLeft();
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    MoveUp();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    MoveDown();
                }
                break;
            case BoardState.TurnState.BlueEnd:
            case BoardState.TurnState.RedEnd:
                break;
        }
    }
}
