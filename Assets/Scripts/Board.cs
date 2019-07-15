using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Column columnPrefab;
    public Column[] columns;
    public GameObject arrow;
    int selectedColumn = 0;
    private BoardState boardState;
    // Start is called before the first frame update
    void Start()
    {
        boardState = new BoardState();
        columns = new Column[21];
        for (int i = 0; i < 21; i++)
        {
            Column col = columns[i] = Instantiate(columnPrefab, transform);
            col.SetState(boardState, i);
            col.transform.localPosition = new Vector3(0.75f * i, 0);
            col.name = string.Format("col{0}", i);
        }
        arrow.transform.localPosition = new Vector3(0, -0.65f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedColumn++;
            selectedColumn %= 21;

            arrow.transform.localPosition = new Vector3(0.75f * selectedColumn, -0.65f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedColumn--;
            if (selectedColumn < 0)
                selectedColumn = 20;

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
