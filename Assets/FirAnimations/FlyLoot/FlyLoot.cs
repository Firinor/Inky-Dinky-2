using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlyLoot : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] 
    private Image image;
    private Vector3 offset;
    [SerializeField] 
    private AnimationCurve moveCurve;
    [SerializeField] 
    private AnimationCurve YCurve;
    [SerializeField]
    private float lyingDuration;
    
    private Vector3 startPosition;
    
    private float elapsedTime;
    private float dropDuration;
    private float flyDuration;

    public Action OnPointerEnterAction;
    private bool secondStep;
    
    public void SetDestination(Sprite goods, Transform endPoint, Vector2 offset = default)
    {
        image.sprite = goods;
        this.offset = offset;
        startPosition = transform.position;
        transform.position = endPoint.position;
        image.transform.position = startPosition;

        flyDuration = moveCurve.keys.Last().time;
        dropDuration = YCurve.keys.Last().time;
    }

    private void Update()
    {
        if (secondStep)
            Retraction();
        else
            Prolapse();
    }

    private void Prolapse()
    {
        elapsedTime += Time.deltaTime;

        float YValue = YCurve.Evaluate(elapsedTime);

        image.transform.position = Vector3.Lerp(
            startPosition, 
            startPosition+offset, 
            elapsedTime / dropDuration
        ) + Vector3.up * YValue;
        
        if (elapsedTime > lyingDuration)
            FlyToEnd();
    }

    private void Retraction()
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / flyDuration;
        if (t >= 1)
        {
            Destroy(gameObject);
            return;
        }
        
        float moveValue = moveCurve.Evaluate(t);
        
        image.transform.position = Vector3.LerpUnclamped(
            startPosition+offset, 
            transform.position, 
            moveValue
        );
    }

    public void FlyToEnd()
    {
        if(secondStep) 
            return;

        image.raycastTarget = false;
        elapsedTime = 0;
        secondStep = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(secondStep) 
            return;
        
        OnPointerEnterAction?.Invoke();
    }
}