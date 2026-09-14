using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryPopper : MonoBehaviour
{
    public static CherryPopper instance;

    public Transform poppaCherry;

    public SpriteRenderer poppaSprite;

    public Sprite neutralSprite;
    public Sprite gasmSprite;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Poppim()
    {
        StartCoroutine(this.PopCherry());
    }

    private IEnumerator PopCherry()
    {
        this.poppaSprite.sprite = gasmSprite;
        this.poppaCherry.DOShakePosition(0.25f, 0.2f);
        yield return new WaitForSeconds(1.0f);
        this.poppaSprite.sprite = neutralSprite;

    }
}
