using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    public Cell cellPrefab;
    public Cell[] cells;
    public float scale = 0.75f;
    public bool isMoving = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetState(BoardState boardState, int column)
    {
        cells = new Cell[boardState.tiles.GetLength(0)];
        for (int i = 0; i < cells.Length; i++)
        {
            Cell cell = cells[i] = Instantiate(cellPrefab, transform);
            cell.IsBG = !(boardState.tiles[i, column] == BoardState.TileType.Wall);
            cell.transform.localPosition = new Vector3(0, scale * i);
            cell.name = string.Format("cell{0}", i);
        }
    }
    public void MoveUp()
    {
        if (isMoving)
            return;
        isMoving = true;
        StartCoroutine(SlowMoveUp());
    }
    IEnumerator SlowMoveUp()
    {
        Cell tmpCell = Instantiate(cells[cells.Length - 1], transform);
        tmpCell.transform.localPosition = new Vector3(0, scale * -1);
        int steps = 25;
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(1 / steps);
            transform.position += new Vector3(0, scale / steps);
        }
        Destroy(tmpCell.gameObject);
        //reposition cells
        transform.position -= new Vector3(0, scale);
        tmpCell = cells[cells.Length - 1];
        for (int i = cells.Length - 1; i > 0; i--)
        {
            cells[i] = cells[i - 1];
        }
        cells[0] = tmpCell;
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].transform.localPosition = new Vector3(0, scale * i);
        }
        isMoving = false;
    }
    public void MoveDown()
    {
        if (isMoving)
            return;
        isMoving = true;
        StartCoroutine(SlowMoveDown());
    }
    IEnumerator SlowMoveDown()
    {
        Cell tmpCell = Instantiate(cells[0], transform);
        tmpCell.transform.localPosition = new Vector3(0, scale * cells.Length);
        int steps = 25;
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(1 / steps);
            transform.position -= new Vector3(0, scale / steps);
        }
        Destroy(tmpCell.gameObject);
        //reposition cells
        transform.position += new Vector3(0, scale);
        tmpCell = cells[0];
        for (int i = 0; i < cells.Length - 1; i++)
        {
            cells[i] = cells[i + 1];
        }
        cells[cells.Length - 1] = tmpCell;
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].transform.localPosition = new Vector3(0, scale * i);
        }
        isMoving = false;
    }
}
