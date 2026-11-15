using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHand : MonoBehaviour
{
    public MainImageManager ImageManager;
    public CameraMover CameraMover;
    public Color[] mainPixels;
    public Transform[] BrushColorPoints;
    public Transform[] FairyColorPoints;
    public PlayerInputHolder PlayerInput;
    public Camera mainCamera;
    public Material MainImageMaterial;
    public Sprite MainSprite;

    public float BigCellZoom;
    public float MiniCellZoom;

    public Image SpellWideBrushImage;
    public Image SpellFillToolImage;
    
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

    private const float HalfAlfa = 0.5f;
    private const byte HalfAlfa32 = 127;
    
    //SPELLS
    private bool IsWideBrush;
    private bool IsFillTool;
    private bool IsPippet;
    
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

    public void SwitchWideBrush()
    {
        IsWideBrush = !IsWideBrush;
        SpellWideBrushImage.color = IsWideBrush ?  Color.aquamarine : Color.white;
    }
    
    public void SwitchFillTool()
    {
        IsFillTool = !IsFillTool;
        SpellFillToolImage.color = IsFillTool ?  Color.aquamarine : Color.white;
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
            isDirty = false;
        }
        if (isHighlight)
        {
            _inGameplayHighlight.SetPixels32(HighlightPixels);
            _inGameplayHighlight.Apply();
            isHighlight = false;
        }
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
        mainPixels = ImageManager.mainPixels;
        InGameSolvedPixels = new Color32[ImageManager.ImageLength];
        for (int i = 0; i < InGameSolvedPixels.Length; i++)
        {
            InGameSolvedPixels[i] = new Color32();
        }
        IsSolvedPixel = new bool[InGameSolvedPixels.Length];
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

        Color32 colorCode = jar.codeColor;
        for (int i = 0; i < mainPixels.Length; i++)
        {
            if (mainPixels[i].a >= HalfAlfa
                && !IsSolvedPixel[i]
                && mainPixels[i].IsEqualColorCode(colorCode))
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
        
        Color32 colorCode = jar.codeColor;
        for (int i = 0; i < mainPixels.Length; i++)
        {
            if (mainPixels[i].IsEqualColorCode(colorCode))
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

        List<Vector2Int> indexes = new List<Vector2Int>{ pixelPos };
        if (IsFillTool)
        {
            indexes = AddAllNeighborsFill(pixelPos);
        }
        else if (IsWideBrush)
        {
            indexes = AddNeighborsWideBrush(indexes);
        }
        
        foreach (var pixel in indexes)
        {
            int i = GetIndex(pixel.x, pixel.y);
            Color32 pixelColor = ImageManager.mainPixels[i];

            if(pixelColor.a <= 0)
                continue;
            if(IsSolvedPixel[i])
                continue;
            if (!IsColorInBrush(mainPixels[i]))
                continue;
            
            InGameSolvedPixels[i] = pixelColor;
            HighlightPixels[i] = new Color32();
            IsSolvedPixel[i] = true;
            isHighlight = true;
            isDirty = true;
            IsPainting = true;
        }
    }
    private bool IsColorInBrush(Color color)
    {
        if(brush.Count <= 0)
            return false;

        foreach (ColorJar colorJar in brush)
        {
            Color32 colorCode = colorJar.codeColor;
            if (color.IsEqualColorCode(colorCode))
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
        if (IsWideBrush)
        {
            line = AddNeighborsWideBrush(line);
        }
        foreach (var pixel in line)
        {
            //Debug.Log(pixel);
            
            if(pixel.x < 0 
               || pixel.y < 0 
               || pixel.x >= ImageManager.ImageWidth 
               || pixel.y >= ImageManager.ImageHeight)
                continue;
            
            int index = GetIndex(pixel.x, pixel.y);
            if(mainPixels[index].a < HalfAlfa)
                continue;
            if(IsSolvedPixel[index])
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
    public List<Vector2Int> AddAllNeighborsFill(Vector2Int startPoint)
    {
        List<Vector2Int> result = new();

        int index = GetIndex(startPoint.x, startPoint.y);
        if (HighlightPixels[index].a >= HalfAlfa32)
        {
            var visited = new bool[ImageManager.ImageWidth, ImageManager.ImageHeight];
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(startPoint);
            visited[startPoint.x, startPoint.y] = true;
        
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = {  0, 0, -1, 1 };
    
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);
        
                for (int i = 0; i < 4; i++)
                {
                    int nx = current.x + dx[i];
                    int ny = current.y + dy[i];
            
                    if (nx < 0 || nx >= ImageManager.ImageWidth || ny < 0 || ny >= ImageManager.ImageHeight) 
                        continue;
                    if (visited[nx, ny]) 
                        continue;
                    index = GetIndex(nx, ny);
                    if (HighlightPixels[index].a < HalfAlfa32) 
                        continue;
            
                    visited[nx, ny] = true;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }
        
        return result;
    }
    
    
    public List<(int x, int y)> GetFloodFill(int x1, int y1, int[,] field, int width, int height)
    {
        int targetColor = field[y1, x1];
        var result = new List<(int x, int y)>();
    
        if (targetColor == 0) return result;
    
        var visited = new bool[height, width];
        var queue = new Queue<(int x, int y)>();
        queue.Enqueue((x1, y1));
        visited[y1, x1] = true;
        
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = {  0, 0, -1, 1 };
    
        while (queue.Count > 0)
        {
            var (cx, cy) = queue.Dequeue();
            result.Add((cx, cy));
        
            for (int i = 0; i < 4; i++)
            {
                int nx = cx + dx[i];
                int ny = cy + dy[i];
            
                if (nx < 0 || nx >= width || ny < 0 || ny >= height) continue;
                if (visited[ny, nx]) continue;
                if (field[ny, nx] != targetColor) continue;
            
                visited[ny, nx] = true;
                queue.Enqueue((nx, ny));
            }
        }
    
        return result;
    }
    
    
    
    
    private List<Vector2Int> AddNeighborsWideBrush(List<Vector2Int> line)
    {
        HashSet<Vector2Int> result = new();
        foreach (var point in line)
        {
            result.Add(point);
            //left
            if (point.x - 1 >= 0)
                result.Add(new Vector2Int(point.x - 1, point.y));
            //right
            if (point.x + 1 < ImageManager.ImageWidth)       
                result.Add(new Vector2Int(point.x + 1, point.y));
            //up
            if (point.y - 1 >= 0)          
                result.Add(new Vector2Int(point.x, point.y - 1));
            //down
            if (point.y + 1 < ImageManager.ImageHeight)      
                result.Add(new Vector2Int(point.x, point.y + 1));
        }
        return result.ToList();
    }

    public void MoveCameraToOpenPixel()
    {
        if(brush.Count <= 0)
            return;

        int pixelIndex = -1;
        for (int i = 0; i < HighlightPixels.Length; i++)
        {
            if(HighlightPixels[i].r < 255)
                continue;

            pixelIndex = i;
            break;
        }

        Vector2Int pos = GetPixelPositionByIndex(pixelIndex);
        int pixelsPerUnit = 100;
        
        Vector3 localPos = new Vector3(
            (pos.x - ImageManager.ImageWidth/2f) / (float)pixelsPerUnit,
            (pos.y - ImageManager.ImageHeight/2f) / (float)pixelsPerUnit,
            0f
        );
        
        localPos.z = -10f;
        //Debug.Log("CameraMover.MoveToPosition: " + pos);
        CameraMover.MoveToPosition(localPos);
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

    private Vector2Int GetPixelPositionByIndex(int i)
    {
        return new Vector2Int(
            i%ImageManager.ImageWidth, //x
            i/ImageManager.ImageWidth);//y
    }
    
    private void OnDestroy()
    {
        PlayerInput.onStartClick -= FindPixels;
        PlayerInput.onEndClick -= ResetPainting;
        PlayerInput.onDrag -= FindPixelsLine;
        PlayerInput.onZoom -= ShowCellOnZoom;
    }
}
