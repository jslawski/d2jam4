using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Sequence removeHealthSequence;
    private Sequence addHealthSequence;

    [SerializeField]
    private RectTransform _barTransform;
    [SerializeField]
    private Image _fillImage;
    [SerializeField]
    private Image _transitionImage;

    private float _timeToDecreaseHealth = 0.2f;

    private Vector3 _originalScale;
    private Vector3 _emphasizeScale;

    private void Awake()
    {
        this._originalScale = this.transform.localScale;
        this._emphasizeScale = this._originalScale * 1.05f;
    }

    public void RemoveHealth(float healthToRemove)
    {
        GameManager.instance.currentHealth = GameManager.instance.currentHealth - healthToRemove;

        if (GameManager.instance.currentHealth <= 0)
        {
            GameManager.instance.currentHealth = 0;
            GameManager.instance.EndGame();
        }

        float targetFillValue = GameManager.instance.currentHealth;
        float currentFillValue = this._fillImage.fillAmount;
        float transitionFillValue = this._fillImage.fillAmount;

        this.removeHealthSequence.Kill();
        this.removeHealthSequence = DOTween.Sequence();

        Tweener shakeTween = this._barTransform.DOShakePosition(this._timeToDecreaseHealth, 20f, 25, 90, false, false);
        Tweener growTween = this._barTransform.DOScale(this._emphasizeScale, 0.2f).SetEase(Ease.OutBack);
        Tweener returnTween = this._barTransform.DOScale(this._originalScale, 0.2f).SetEase(Ease.OutBack);

        this.removeHealthSequence.Append(growTween)
        .Insert(0.0f, shakeTween)
        .Insert(0.0f, DOTween.To(() => currentFillValue, x => currentFillValue = x, targetFillValue, this._timeToDecreaseHealth).SetLink(this.gameObject).OnUpdate(() => { this._fillImage.fillAmount = currentFillValue; }))
        .AppendInterval(0.1f)
        .Append(returnTween)
        .AppendInterval(0.2f)
        .Append(DOTween.To(() => transitionFillValue, x => transitionFillValue = x, targetFillValue, this._timeToDecreaseHealth).SetLink(this.gameObject).OnUpdate(() => { this._transitionImage.fillAmount = transitionFillValue; }));
        

        this.removeHealthSequence.Play();
    }

    public void AddHealth(float healthToAdd)
    {
        GameManager.instance.currentHealth = GameManager.instance.currentHealth + healthToAdd;

        

        if (GameManager.instance.currentHealth > 1.0f)
        {
            GameManager.instance.currentHealth = 1.0f;
        }

        float targetFillValue = GameManager.instance.currentHealth;
        float currentFillValue = this._fillImage.fillAmount;

        this.addHealthSequence.Kill();
        this.addHealthSequence = DOTween.Sequence();

        Tweener growTween = this._barTransform.DOScale(this._emphasizeScale, 0.2f).SetEase(Ease.OutBack);
        Tweener returnTween = this._barTransform.DOScale(this._originalScale, 0.2f).SetEase(Ease.OutBack);

        this.addHealthSequence.Append(growTween)
        .Insert(0.0f, DOTween.To(() => currentFillValue, x => currentFillValue = x, targetFillValue, this._timeToDecreaseHealth).SetLink(this.gameObject).OnUpdate(() => { this._fillImage.fillAmount = currentFillValue; }))
        .Insert(0.0f, DOTween.To(() => currentFillValue, x => currentFillValue = x, targetFillValue, this._timeToDecreaseHealth).SetLink(this.gameObject).OnUpdate(() => { this._transitionImage.fillAmount = currentFillValue; }))
        .Append(returnTween);

        this.addHealthSequence.Play();
    }
}
