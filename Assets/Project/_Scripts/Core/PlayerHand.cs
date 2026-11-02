using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHand : MonoBehaviour
{
    public MainImageManager ImageManager;
    public Color32[] mainPixels;
    public Transform[] BrushColorPoints;
    public PlayerInputHolder PlayerInput;

    public Material MainImageMaterial;
    public Sprite MainSprite;

    public bool IsPainting { get; private set; }
    
    private Color32[] HighlightPixels;
    private Texture2D InGameTexture2D;
    private Color32[] InGamePixels;
    private Texture2D _inGameplayHighlight;
    
    private static readonly int HighlightID = Shader.PropertyToID("Highlight");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private InputActionAsset action;
    private Vector2Int lastMousePosition;

    public void Initialize()
    {
        MainSprite = ImageManager.MainImageSprite;
        InGameTexture2D = ImageManager.InGameTexture2D;
        InGamePixels = InGameTexture2D.GetPixels32();
        mainPixels = ImageManager.mainPixels;
        
        CreateHighlight();
        MainImageMaterial.SetTexture(HighlightID, _inGameplayHighlight);
        
        PlayerInput.onStartClick += FindPixels;
        PlayerInput.onDrag += FindPixelsLine;
    }

    private void CreateHighlight()
    {
        _inGameplayHighlight = new Texture2D(ImageManager.ImageWidth, ImageManager.ImageHeight, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };
        HighlightPixels = new Color32[ImageManager.ImageLength];
        for (int i = 0; i < HighlightPixels.Length; i++)
        {
            HighlightPixels[i] = new Color32();
        }

        _inGameplayHighlight.SetPixels32(HighlightPixels);
        _inGameplayHighlight.Apply();
    }
    
    public bool TryAddColorToBrush(ColorJar jar)
    {
        Transform emptyPoint = BrushColorPoints.FirstOrDefault(p => p.childCount == 0);
        if (emptyPoint is null)
            return false;

        jar.transform.SetParent(emptyPoint);
        jar.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        jar.Button.onClick.RemoveAllListeners();
        jar.Button.onClick.AddListener(() => RemoveColorFromBrush(jar));

        Color32 colorID = jar.ColorImage.color;
        for (int i = 0; i < mainPixels.Length; i++)
        {
            if (mainPixels[i].IsEqualTo(colorID))
            {
                HighlightPixels[i] = new Color32(255, 255, 255, 255);
            }
        }
        _inGameplayHighlight.SetPixels32(HighlightPixels);
        _inGameplayHighlight.Apply();
        MainImageMaterial.SetTexture(HighlightID, _inGameplayHighlight);
        
        return true;
    }

    private void RemoveColorFromBrush(ColorJar jar)
    {
        jar.transform.SetParent(ImageManager.ColorJarParent);
        jar.transform.SetAsFirstSibling();
        jar.Button.onClick.RemoveAllListeners();
        jar.Button.onClick.AddListener(() => TryAddColorToBrush(jar));
        
        Color32 colorID = jar.ColorImage.color;
        for (int i = 0; i < mainPixels.Length; i++)
        {
            if (mainPixels[i].IsEqualTo(colorID))
            {
                HighlightPixels[i] = new Color32(0, 0, 0, 0);
            }
        }
        _inGameplayHighlight.SetPixels32(HighlightPixels);
        _inGameplayHighlight.Apply();
        MainImageMaterial.SetTexture(HighlightID, _inGameplayHighlight);
    }
    
    private void FindPixels(Vector2 pos)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldMousePosition = Camera.main!.ScreenToWorldPoint(mousePosition);
            
        if(!ImageManager.mainImage.sprite.bounds.Contains(worldMousePosition))
           return;
       
        Sprite sprite = ImageManager.mainImage.sprite;
        Vector2 localPos = ImageManager.mainImage.transform.InverseTransformPoint(worldMousePosition);
        localPos *= 100; // Texture scale
        localPos += sprite.pivot;

        if (localPos.x < 0
            || localPos.y < 0
            || localPos.x >= sprite.rect.width
            || localPos.y >= sprite.rect.height)
            return;
        
        Vector2Int pixelPos = new Vector2Int((int)localPos.x, (int)localPos.y);
        Color32 pixelColor = ImageManager.mainPixels[pixelPos.x + pixelPos.y * ImageManager.ImageWidth];

        if (pixelColor.a > 0)
        {
            InGamePixels[pixelPos.x + pixelPos.y * ImageManager.ImageWidth] = pixelColor;
            InGameTexture2D.SetPixels32(InGamePixels);
            InGameTexture2D.Apply();
            lastMousePosition = pixelPos;
        }
    }

    public bool IsPixelPosition(Vector2 pos, out Vector2Int pixelPosition)
    {
        pixelPosition = new(-1, -1);
        Vector2 worldMousePosition = Camera.main!.ScreenToWorldPoint(pos);
        Vector2 localPos = ImageManager.mainImage.transform.InverseTransformPoint(worldMousePosition);
        Debug.Log("pixel coord before: " + localPos);
        localPos *= 100; // Texture scale
        localPos += MainSprite.pivot;

        Debug.Log("pixel coord after: " + localPos);
            
        if (localPos.x < 0
            || localPos.y < 0
            || localPos.x >= MainSprite.rect.width
            || localPos.y >= MainSprite.rect.height)
            return false;
        
        pixelPosition = new Vector2Int((int)localPos.x, (int)localPos.y);
        return true;
    }
    
    private void FindPixelsLine(Vector2 obj)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        
        Debug.Log("mouse:" + mousePosition);
        Debug.Log(obj);
        
        if(!IsPixelPosition(mousePosition, out var endPixel) 
           || !IsPixelPosition(mousePosition - obj, out var startPixel)) 
           return;

        Debug.Log("startPixel" + startPixel);
        Debug.Log("endPixel" + endPixel);
        
        List<Vector2Int> line = Bresenham(startPixel, endPixel);
        foreach (var pixel in line)
        {
            Debug.Log("result:" + pixel);
        }
    }
    
    private List<Vector2Int> Bresenham(Vector2 startPoint, Vector2 endPoint)
    {
        var pixels = new List<Vector2Int>();

        int startX = Mathf.FloorToInt(startPoint.x);
        int startY = Mathf.FloorToInt(startPoint.y);
        int endX = Mathf.FloorToInt(endPoint.x);
        int endY = Mathf.FloorToInt(endPoint.y);

        int deltaX = Mathf.Abs(endX - startX);
        int deltaY = Mathf.Abs(endY - startY);
        int stepX = startX < endX ? 1 : -1;
        int stepY = startY < endY ? 1 : -1;
        int error = deltaX - deltaY;

        int x = startX;
        int y = startY;

        while (true)
        {
            pixels.Add(new Vector2Int(x, y));

            if (x == endX && y == endY)
            {
                break;
            }

            int doubledError = 2 * error;

            bool shouldMoveHorizontally = doubledError > -deltaY;
            bool shouldMoveVertically = doubledError < deltaX;
            
            if (shouldMoveHorizontally && shouldMoveVertically)
            {
                if (Mathf.Abs(doubledError - deltaX) < Mathf.Abs(doubledError + deltaY))
                {
                    error -= deltaY;
                    x += stepX;
                }
                else
                {
                    error += deltaX;
                    y += stepY;
                }
            }
            else if (shouldMoveHorizontally)
            {
                error -= deltaY;
                x += stepX;
            }
            else if (shouldMoveVertically)
            {
                error += deltaX;
                y += stepY;
            }
        }

        return pixels;
    }
    
    private void OnDestroy()
    {
        PlayerInput.onClick -= FindPixels;
    }
}
