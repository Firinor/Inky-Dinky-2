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
    
    private Color32[] HighlightPixels;
    private Texture2D InGameTexture2D;
    private Color32[] InGamePixels;
    private Texture2D _inGameplayHighlight;
    
    private static readonly int HighlightID = Shader.PropertyToID("Highlight");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private InputActionAsset action;
    private Vector2 lastMousePosition;
    private Vector2 gemInHandOffset;
    

    public void Initialize()
    {
        InGameTexture2D = ImageManager.InGameTexture2D;
        InGamePixels = InGameTexture2D.GetPixels32();
        mainPixels = ImageManager.mainPixels;
        CheateHighlight();
        MainImageMaterial.SetTexture(HighlightID, _inGameplayHighlight);
        
        PlayerInput.onClick += FindPixels;
    }
    
    private void CheateHighlight()
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

    private void Update()
    {
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        lastMousePosition = currentMousePosition;
    }
    
    private void FindPixels(Vector2 pos)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldMousePosition = Camera.main!.ScreenToWorldPoint(mousePosition);
            
       if(!ImageManager.mainImage.sprite.bounds.Contains(worldMousePosition)
           || !isMouseOnPoint(worldMousePosition))
           return;

        gemInHandOffset = Camera.main!.WorldToScreenPoint(ImageManager.mainImage.transform.position);
    }

    private bool isMouseOnPoint(Vector3 mousePosition)
    {
        Sprite sprite = ImageManager.mainImage.sprite;
        Vector2 localPos = ImageManager.mainImage.transform.InverseTransformPoint(mousePosition);
        localPos *= 100; // Texture scale
        localPos += sprite.pivot;

        if (localPos.x < 0
            || localPos.y < 0
            || localPos.x >= sprite.rect.width
            || localPos.y >= sprite.rect.height)
            return false;
        
        Vector2 spritePos = sprite.rect.position;
        Vector2Int pixelPos = new Vector2Int((int)(spritePos.x + localPos.x), (int)(spritePos.y + localPos.y));
        Color32 pixelColor = ImageManager.mainPixels[pixelPos.x + pixelPos.y * ImageManager.ImageWidth];

        if (pixelColor.a > 0)
        {
            InGamePixels[pixelPos.x + pixelPos.y * ImageManager.ImageWidth] = pixelColor;
            InGameTexture2D.SetPixels32(InGamePixels);
            InGameTexture2D.Apply();
            //MainImageMaterial.SetTexture(MainTex, InGameTexture2D);
            return true;
        }
        else
            return false;
    }
    
    private void OnDestroy()
    {
        PlayerInput.onClick -= FindPixels;
    }
}
