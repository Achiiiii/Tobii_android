using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Test2ButtonTrigger : MonoBehaviour
{
    Button btn;
    Transform transform;
    RectTransform rect;
    public AudioSource audioSource;
    private bool _isEnter;
    private bool _isScaleReset = true;

    // 視點圖片所在的 Layer（在 Inspector 中設定）
    public LayerMask gazeCursorLayer;
    private BoxCollider2D boxCollider;

    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
        btn = gameObject.GetComponent<Button>();
        transform = gameObject.GetComponent<Transform>();
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        float pivotX = rect.pivot.x;
        float pivotY = rect.pivot.y;
        float offsetX = 0;
        float offsetY = 0;
        switch (pivotX)
        {
            case 0:
                offsetX = rect.sizeDelta.x / 2;
                break;
            case 0.5f:
                offsetX = 0;
                break;
            case 1:
                offsetX = rect.sizeDelta.x / 2 * -1;
                break;
        }
        switch (pivotY)
        {
            case 0:
                offsetY = rect.sizeDelta.y / 2;
                break;
            case 0.5f:
                offsetY = 0;
                break;
            case 1:
                offsetY = rect.sizeDelta.y / 2 * -1;
                break;
        }
        boxCollider.offset = new Vector2(offsetX, offsetY);
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y);
        btn.onClick.AddListener(ClickAudio);
    }

    void Update()
    {
        if (boxCollider == null) return;

        // 計算 Collider 的世界位置（考慮 offset）
        Vector2 worldCenter = (Vector2)transform.position + boxCollider.offset;
        Collider2D hit = Physics2D.OverlapBox(worldCenter, boxCollider.size, 0f, gazeCursorLayer);

        bool isOverlapping = hit != null;

        if (isOverlapping && !_isEnter)
        {
            // 視點圖片進入（包含生成時已在範圍內的情況）
            OnTriggerEnter2D(hit);
        }
        else if (!isOverlapping && _isEnter)
        {
            // 視點圖片離開
            OnTriggerExit2D(null);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        _isEnter = true;
        // if (_isScaleReset)
        // {
        //     _isScaleReset = false;
        //     transform.DOScale(1.2f, 3).SetEase(Ease.OutCubic).OnComplete(() =>
        //     {
        //         transform.DOScale(1, 0);
        //         btn.onClick.Invoke();
        //         _isScaleReset = true;
        //     });
        // }
        btn.onClick.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _isEnter = false;
        // StartCoroutine(DelayExit());
    }

    // private IEnumerator DelayExit()
    // {
    //     yield return new WaitForSeconds(0.2f);
    //     if (!_isEnter)
    //     {
    //         transform.DOKill();
    //         transform.DOScale(1, 0);
    //         _isScaleReset = true;
    //     }
    // }
    private void ClickAudio()
    {
        if (audioSource)
            audioSource.Play();
    }
}
