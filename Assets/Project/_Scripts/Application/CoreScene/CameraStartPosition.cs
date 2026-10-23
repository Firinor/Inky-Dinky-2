using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraStartPosition : MonoBehaviour
{
    public float XpositionSoliter = 0;
    public float XpositionCollecting = 2;
    public float YpositionSoliter = 0;
    public float YpositionCollecting = 2;
    public float solitarePortraitOrthographicSize = 5;
    public float solitareLandscapeOrthographicSize = 10;
    public float collectingPortraitOrthographicSize = 5;
    public float collectingLandscapeOrthographicSize = 10;
    
    public void Initialize(SaveData player)
    {
        bool isLandscape = Screen.width > Screen.height;
        Vector3 cameraPosition = transform.position;

        if (player.GameMode == GameMode.Solitare)
        {
            cameraPosition.x = XpositionSoliter;
            cameraPosition.y = YpositionSoliter;
            if (isLandscape)
                Camera.main.orthographicSize = solitareLandscapeOrthographicSize;
            else
                Camera.main.orthographicSize = solitarePortraitOrthographicSize;
        }
        else if (player.GameMode == GameMode.Collecting)
        {
            cameraPosition.x = XpositionCollecting;
            cameraPosition.y = YpositionCollecting;
            if (isLandscape)
                Camera.main.orthographicSize = collectingLandscapeOrthographicSize;
            else
                Camera.main.orthographicSize = collectingPortraitOrthographicSize;
        }

        transform.position = cameraPosition;
    }
}
