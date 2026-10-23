using System;
using System.Collections;
using UnityEngine;

public class CanvasRatioManager : MonoBehaviour
{
    public static event Action onChange;
    private Vector2Int lastScreen;
    private readonly WaitForSeconds delay = new(5f);

    private void Start()
    {
        lastScreen = new (){ x = Screen.width, y = Screen.height };
        StartCoroutine(CheckUpdates());
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator CheckUpdates()
    {
        while (true)
        {
            yield return delay;
            if (Screen.height == lastScreen.y
                && Screen.width == lastScreen.x) 
                continue;
            
            lastScreen = new Vector2Int { x = Screen.width, y = Screen.height };
            onChange?.Invoke();
        }
    }
}
