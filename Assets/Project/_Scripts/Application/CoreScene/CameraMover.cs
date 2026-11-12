using System.Collections;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private PlayerHand playerHand;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private float XBorder;
    [SerializeField] private float YBorder;
    [SerializeField] private PlayerInputHolder input;

    [SerializeField] private float inertia;
    [SerializeField] private float autoSpeed;

    private Vector2 currentInertia;
    public Coroutine moveCoroutine;
    private Vector3 moveCoroutineTarget;
    
    public void Initialize()
    {
        input.onClick += OnClick;
        input.onDrag += OnDrag;
        input.onZoom += OnZoom;
    }

    private void OnClick(Vector2 pos)
    {
        CheckCoroutine();
    }

    private void CheckCoroutine()
    {
        if (moveCoroutine is null)
            return;
        
        StopAllCoroutines();
        moveCoroutine = null;
    }
    private void OnZoom(float delta)
    {
        float scaleFactor = 1.0f + delta * zoomSpeed;
        float newScale = mainCamera.orthographicSize * scaleFactor;
        newScale = Mathf.Clamp(newScale, minZoom, maxZoom);
        mainCamera.orthographicSize = newScale;
    }

    private void OnDrag(Vector2 delta)
    {
        CheckCoroutine();
        
        if(playerHand.IsPainting)
            return;
        
        Vector3 camPosition = mainCamera.transform.position;
        Vector3 fromDeltaPosition = mainCamera.ScreenToWorldPoint(Vector3.zero);
        Vector3 toDeltaPosition = mainCamera.ScreenToWorldPoint(delta);
        Vector3 vector3Delta = fromDeltaPosition - toDeltaPosition;
        camPosition += vector3Delta;
        camPosition.x = Mathf.Max(startPosition.x-XBorder, camPosition.x);
        camPosition.x = Mathf.Min(startPosition.x+XBorder, camPosition.x);
        camPosition.y = Mathf.Max(startPosition.y-YBorder, camPosition.y);
        camPosition.y = Mathf.Min(startPosition.y+YBorder, camPosition.y);
        mainCamera.transform.position = camPosition;
    }

    public void MoveToPosition(Vector3 position)
    {
        if(moveCoroutine is not null)
            return;

        moveCoroutineTarget = position;
        moveCoroutine = StartCoroutine(MoveToPositionCoroutine());
    }

    private IEnumerator MoveToPositionCoroutine()
    {
        while (Vector3.Distance(mainCamera.transform.position, moveCoroutineTarget) > 0.01f)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, moveCoroutineTarget, Time.deltaTime * autoSpeed);
            yield return null;
        }    
    }   

    private void OnDestroy()
    {
        input.onDrag -= OnDrag;
        input.onZoom -= OnZoom;
    }
}
