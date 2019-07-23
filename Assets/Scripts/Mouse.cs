using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public enum Faction
    {
        Blue,
        Red
    }
    public Faction faction;
    public IEnumerator MoveToCell(Cell targetCell, float targetTime)
    {
        transform.SetParent(targetCell.transform);
        if (targetTime == 0f)
        {
            transform.position = targetCell.transform.position;
        }
        else
        {
            yield return StartCoroutine(MoveToPosition(targetCell, targetTime));
        }
    }
    IEnumerator MoveToPosition(Cell targetCell, float targetTime)
    {
        var elapsedTime = 0f;
        var startPos = transform.position;
        var targetPos = targetCell.transform.position;
        if (targetPos != startPos)
        while (elapsedTime < targetTime)
        {
            //print(elapsedTime);
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, (elapsedTime / targetTime));
            yield return null;
        }
    }
}
