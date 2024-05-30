using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Interaction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Vector3 baseScale;
    [SerializeField] private Vector3 offSetScale;
    [SerializeField] private float time = 5;
    private void Start()
    {
        baseScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(offSetScale, time);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(baseScale, time);
    }
}
