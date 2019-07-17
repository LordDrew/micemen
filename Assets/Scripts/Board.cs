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
    private Mouse[] blueMice;
    private Mouse[] redMice;
    private GameObject[] possibleTurnsIndicators;
    // Start is called before the first frame update
    void Start()
    {
        boardState = new BoardState();
        columns = new Column[boardState.tiles.GetLength(1)];
        possibleTurnsIndicators = new GameObject[columns.Length];
        blueMice = new Mouse[12];
        redMice = new Mouse[12];
        int blueMiceCount = 0;
        int redMiceCount = 0;
        for (int i = 0; i < columns.Length; i++)
        {
            Column col = columns[i] = Instantiate(columnPrefab, transform);
            col.SetState(boardState, i);
            col.transform.localPosition = new Vector3(0.75f * i, 0);
            col.name = string.Format("col{0}", i);
            var turnIndicator = possibleTurnsIndicators[i] = Instantiate(turnIndicatorPrefab, transform);
            turnIndicator.transform.localPosition = new Vector3(0.75f * i, -0.65f);
            for (int j = 0; j < col.cells.Length; j++)
            {
                if (boardState.tiles[j, i] == BoardState.TileType.BlueMouse)
                {
                    var mouse = blueMice[blueMiceCount++] = Instantiate(blueMousePrefab, transform);
                    mouse.transform.position = col.cells[j].transform.position;
                    mouse.transform.SetParent(col.cells[j].transform);
                }
                else if (boardState.tiles[j, i] == BoardState.TileType.RedMouse)
                {
                    var mouse = redMice[redMiceCount++] = Instantiate(redMousePrefab, transform);
                    mouse.transform.position = col.cells[j].transform.position;
                    mouse.transform.SetParent(col.cells[j].transform);
                }
            }
        }
        UpdatePossibleTurns();
    }

    private void UpdatePossibleTurns()
    {
        for (int i = 0; i < possibleTurnsIndicators.Length; i++)
        {
            possibleTurnsIndicators[i].SetActive(false);
        }
        for (int i = 0; i < boardState.validTurns.Count; i++)
        {
            possibleTurnsIndicators[boardState.validTurns[i]].SetActive(true);
        }
        selectedTurn = 0;
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
            UpdatePossibleTurns();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            columns[boardState.validTurns[selectedTurn]].MoveDown();
            boardState.MoveDown(boardState.validTurns[selectedTurn]);
            UpdatePossibleTurns();
        }
    }
}
