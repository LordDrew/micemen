﻿using System.Collections;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Animator>().SetFloat("Offset", Random.Range(0.0f, 1.0f));
    }
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
        if (startPos.y > targetPos.y)
            targetTime /= 2;
        if (targetPos != startPos)
        {
            while (elapsedTime < targetTime)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, targetPos, (elapsedTime / targetTime));
                yield return null;
            }
        }
    }
}
