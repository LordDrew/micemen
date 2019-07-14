using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    public Cell cellPrefab;
    public Cell[] cells;
    // Start is called before the first frame update
    void Start()
    {
        cells = new Cell[13];
        for (int i = 0; i < 13; i++)
        {
            Cell cell = cells[i] = Instantiate(cellPrefab, transform);
            cell.IsBG = Random.Range(0, 2) == 0;
            cell.transform.localPosition = new Vector3(0, 0.75f * i);
            cell.name = string.Format("cell{0}", i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
