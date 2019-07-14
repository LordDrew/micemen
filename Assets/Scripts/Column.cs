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
        cells = new Cell[13];
        for (int i = 0; i < 13; i++)
        {
            Cell cell = cells[i] = Instantiate(cellPrefab, transform);
            cell.IsBG = Random.Range(0, 2) == 0;
            cell.transform.localPosition = new Vector3(0, scale * i);
            cell.name = string.Format("cell{0}", i);
        }
    }

    // Update is called once per frame
    void Update()
    {

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
        Cell tmpCell = Instantiate(cells[12], transform);
        tmpCell.transform.localPosition = new Vector3(0, scale * -1);
        //tmpCell.transform.rotation = cells[12].transform.rotation;
        int steps = 25;
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(1 / steps);
            transform.position += new Vector3(0, scale / steps);
        }
        Destroy(tmpCell.gameObject);
        //reposition cells
        transform.position -= new Vector3(0, scale);
        tmpCell = cells[12];
        for (int i = 12; i > 0; i--)
        {
            cells[i] = cells[i - 1];
        }
        cells[0] = tmpCell;
        for (int i = 0; i < 13; i++)
        {
            cells[i].transform.localPosition = new Vector3(0, scale * i);
        }
        isMoving = false;
    }
}
