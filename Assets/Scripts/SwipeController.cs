using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [SerializeField] private int maxPage = 3;
    private int currentPage;
    private Vector3 targetPos;

    [SerializeField] private Vector3 pageStep;
    [SerializeField] private RectTransform levelPagesRect;

    [SerializeField] private float tweenTime = 0.5f;
    [SerializeField] private LeanTweenType tweenType = LeanTweenType.easeInOutQuad;

    private void Awake()
    {
        currentPage = 1;
        targetPos = levelPagesRect.localPosition;
    }

    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }

    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
        }
    }

    private void MovePage()
    {
        LeanTween.moveLocal(levelPagesRect.gameObject, targetPos, tweenTime).setEase(tweenType);
    }
}
