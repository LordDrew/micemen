using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Column columnPrefab;
    public Column[] columns;
    public GameObject arrow;
    public Mouse blueMousePrefab;
    public Mouse redMousePrefab;
    int selectedColumn = 0;
    private BoardState boardState;
    private Mouse[] blueMice;
    private Mouse[] redMice;
    // Start is called before the first frame update
    void Start()
    {
        boardState = new BoardState();
        columns = new Column[boardState.tiles.GetLength(1)];
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
        arrow.transform.localPosition = new Vector3(0, -0.65f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedColumn++;
            selectedColumn %= columns.Length;

            arrow.transform.localPosition = new Vector3(0.75f * selectedColumn, -0.65f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedColumn--;
            if (selectedColumn < 0)
                selectedColumn = columns.Length - 1;

            arrow.transform.localPosition = new Vector3(0.75f * selectedColumn, -0.65f);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            columns[selectedColumn].MoveUp();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            columns[selectedColumn].MoveDown();
        }
    }
}
