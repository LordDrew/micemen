using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Column columnPrefab;
    public Column[] columns;
    // Start is called before the first frame update
    void Start()
    {
        columns = new Column[21];
        for (int i = 0; i < 21; i++)
        {
            Column col = columns[i] = Instantiate(columnPrefab, transform);
            col.transform.localPosition = new Vector3(0.75f * i, 0);
            col.name = string.Format("col{0}", i);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
