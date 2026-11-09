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
    public Camera mainCamera;
    public Material MainImageMaterial;
    public Sprite MainSprite;

    public float BigCellZoom;
    public float MiniCellZoom;
    
    public bool IsPainting { get; private set; }
    
    private Color32[] HighlightPixels;
    private Color32[] InGameSolvedPixels;
    private bool[] IsSolvedPixel;
    private Texture2D _inGameplayHighlight;
    private Texture2D InGameSolvedTexture2D;
    
    private static readonly int HighlightID = Shader.PropertyToID("Highlight");
    private static readonly int SolvedTexID = Shader.PropertyToID("_solvedPixels");
    private static readonly int MaxCellID = Shader.PropertyToID("_MaxCell");
    private static readonly int MinCellID = Shader.PropertyToID("_MinCell");
    private Vector2 lastMousePosition;

    private readonly List<ColorJar> brush = new(5);

    private bool isDirty;
    private bool isHighlight;
    
    public void Initialize()
    {
        MainSprite = ImageManager.MainImageSprite;
        
        CreateHighlight();
        CreateSolvedTexture();
        MainImageMaterial.SetTexture(HighlightID, _inGameplayHighlight);
        MainImageMaterial.SetTexture(SolvedTexID, InGameSolvedTexture2D);
        MainImageMaterial.SetFloat(MaxCellID, mainCamera.orthographicSize > BigCellZoom ? 0 : 1);
        MainImageMaterial.SetFloat(MinCellID, mainCamera.orthographicSize > MiniCellZoom ? 0 : 1);
        
        PlayerInput.onStartClick += FindPixels;
        PlayerInput.onEndClick += ResetPainting;
        PlayerInput.onDrag += FindPixelsLine;
        PlayerInput.onZoom += ShowCellOnZoom;
    }

    private void ShowCellOnZoom(float deltaZoom)
    {
        float cameraSize = mainCamera.orthographicSize;
        MainImageMaterial.SetFloat(MaxCellID, cameraSize > BigCellZoom ? 0 : 1);
        MainImageMaterial.SetFloat(MinCellID, cameraSize > MiniCellZoom ? 0 : 1);
    }

    private void Update()
    {
        if (isDirty)
        {
            InGameSolvedTexture2D.SetPixels32(InGameSolvedPixels);
            InGameSolvedTexture2D.Apply();
        }
        if (isHighlight)
        {
            _inGameplayHighlight.SetPixels32(HighlightPixels);
            _inGameplayHighlight.Apply();
        }

        isHighlight = false;
        isDirty = false;
    }

    private void ResetPainting() => IsPainting = false;

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
        isHighlight = true;
    }
    private void CreateSolvedTexture()
    {
        InGameSolvedTexture2D = new Texture2D(ImageManager.ImageWidth, ImageManager.ImageHeight, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };
        InGameSolvedPixels = new Color32[ImageManager.ImageLength];
        for (int i = 0; i < InGameSolvedPixels.Length; i++)
        {
            InGameSolvedPixels[i] = new Color32();
        }
        IsSolvedPixel = new bool[InGameSolvedPixels.Length];
        mainPixels = ImageManager.mainPixels;
        isDirty = true;
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
            if (mainPixels[i].IsEqualTo(colorID)
                && !IsSolvedPixel[i])
                HighlightPixels[i] = new Color32(255, 255, 255, 255);
        }
        brush.Add(jar);
        
        isHighlight = true;
        return true;
    }
    private void RemoveColorFromBrush(ColorJar jar)
    {
        jar.Button.onClick.RemoveAllListeners();
        jar.Button.onClick.AddListener(() => TryAddColorToBrush(jar));
        if (jar.Count > 0)
        {
            jar.transform.SetParent(ImageManager.ColorJarParent);
            jar.transform.SetAsFirstSibling();
        }
        else
        {
            Destroy(jar.gameObject);
        }
        
        Color32 colorID = jar.ColorImage.color;
        for (int i = 0; i < mainPixels.Length; i++)
        {
            if (mainPixels[i].IsEqualTo(colorID))
            {
                HighlightPixels[i] = new Color32(0, 0, 0, 0);
            }
        }
        isHighlight = true;
        brush.Remove(jar);
    }
    
    private void FindPixels(Vector2 pos)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        lastMousePosition = mousePosition;
        Vector2 localPos = GetPixelPosition(mousePosition);
        if (localPos.x < 0
            || localPos.y < 0
            || localPos.x >= ImageManager.ImageWidth
            || localPos.y >= ImageManager.ImageHeight)
            return;
        
        Vector2Int pixelPos = new Vector2Int((int)localPos.x, (int)localPos.y);
        int index = GetIndex(pixelPos.x, pixelPos.y);
        Color32 pixelColor = ImageManager.mainPixels[index];

        if (pixelColor.a <= 0)
            return;
        if(IsSolvedPixel[index])
            return;
        if (!IsColorInBrush(mainPixels[index]))
            return;
        
        InGameSolvedPixels[index] = pixelColor;
        HighlightPixels[index] = new Color32();
        IsSolvedPixel[index] = true;
        isHighlight = true;
        isDirty = true;
        IsPainting = true;
    }

    private bool IsColorInBrush(Color32 color)
    {
        if(brush.Count <= 0)
            return false;

        foreach (ColorJar colorJar in brush)
        {
            Color32 jarColor = colorJar.ColorImage.color;
            if (jarColor.IsEqualTo(color))
            {
                colorJar.Count--;
                colorJar.TextCount.text = colorJar.Count.ToString();
                colorJar.SliderImage.fillAmount = colorJar.Count/colorJar.StartCount;
                if(colorJar.Count <= 0)
                    RemoveColorFromBrush(colorJar);
                return true;
            }
        }

        return false;
    }

    private Vector2Int GetPixelPosition(Vector2 pos)
    {
        Vector2 worldMousePosition = Camera.main!.ScreenToWorldPoint(pos);
        Vector2 localPos = ImageManager.mainImage.transform.InverseTransformPoint(worldMousePosition);
        localPos *= 100; // Texture scale
        localPos += MainSprite.pivot;
        return new Vector2Int((int)localPos.x, (int)localPos.y);
    }
    
    private void FindPixelsLine(Vector2 obj)
    {
        if(brush.Count <= 0)
            return;
        if(!IsPainting)
            return;
        
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        //Debug.Log("mouse:" + mousePosition + " lastMousePosition" + lastMousePosition);
        
        Vector2 endPixel = GetPixelPosition(mousePosition);
        Vector2 startPixel = GetPixelPosition(lastMousePosition);

        lastMousePosition = mousePosition;
        //Debug.Log("new lastMousePosition " + lastMousePosition);
        //Debug.Log("Start " +  startPixel + " end " + endPixel);
        List<Vector2Int> line = Bresenham(startPixel, endPixel);
        foreach (var pixel in line)
        {
            //Debug.Log(pixel);
            
            if(pixel.x < 0 
               || pixel.y < 0 
               || pixel.x >= ImageManager.ImageWidth 
               || pixel.y >= ImageManager.ImageHeight)
                continue;
            
            int index = GetIndex(pixel.x, pixel.y);
            if(IsSolvedPixel[index])
                continue;
            if(mainPixels[index].a <= 0)
                continue;
            if (!IsColorInBrush(mainPixels[index]))
                continue;
            
            InGameSolvedPixels[index] = mainPixels[index];
            HighlightPixels[index] = new Color32();
            IsSolvedPixel[index] = true;
            isHighlight = true;
            isDirty = true;
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

    private int GetIndex(int x, int y)
    {
        return x + y * ImageManager.ImageWidth;
    }
    
    private void OnDestroy()
    {
        PlayerInput.onStartClick -= FindPixels;
        PlayerInput.onEndClick -= ResetPainting;
        PlayerInput.onDrag -= FindPixelsLine;
        PlayerInput.onZoom -= ShowCellOnZoom;
    }
}
