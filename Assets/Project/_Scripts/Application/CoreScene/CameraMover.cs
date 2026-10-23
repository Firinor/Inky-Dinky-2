using Unity.Mathematics;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private float XBorder;
    [SerializeField] private float YBorder;
    [SerializeField] private PlayerInputHolder input;

    [SerializeField] private float inertia;

    private Vector2 currentInertia;
    
    public void Initialize()
    {
        input.onDrag += OnDrag;
        input.onZoom += OnZoom;
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

    private void OnDestroy()
    {
        input.onDrag -= OnDrag;
        input.onZoom -= OnZoom;
    }
}
