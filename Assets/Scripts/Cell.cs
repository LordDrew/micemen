using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private bool _is_bg = true;
    public bool IsBG
    {
        get { return _is_bg; }
        set
        {
            if (value == _is_bg)
                return;
            _is_bg = value;
            GetComponent<SpriteRenderer>().sprite = value ? bgSprite : fgSprite;
        }
    }
    public Sprite bgSprite;
    public Sprite fgSprite;

    public void SetBG(bool bg)
    {

    }
}
