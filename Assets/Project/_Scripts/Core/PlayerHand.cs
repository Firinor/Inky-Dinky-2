using System.Linq;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public MainImageManager ImageManager;
    public Color32[] mainPixels;
    public Transform[] BrushColorPoints;
    public PlayerInputHolder PlayerInput;

    public Material MainImageMaterial;
    
    private Color32[] playerProgressPixels;
    private Texture2D _inGameplayHighlight;
    
    private static readonly int HighlightID = Shader.PropertyToID("Highlight");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    public void Initialize()
    {
        mainPixels = ImageManager.mainPixels;
        CheateHighlight();
        MainImageMaterial.SetTexture(HighlightID, _inGameplayHighlight);
    }
    
    private void CheateHighlight()
    {
        _inGameplayHighlight = new Texture2D(ImageManager.ImageWidth, ImageManager.ImageHeight, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };
        playerProgressPixels = new Color32[ImageManager.ImageLength];
        for (int i = 0; i < playerProgressPixels.Length; i++)
        {
            playerProgressPixels[i] = new Color32();
        }

        _inGameplayHighlight.SetPixels32(playerProgressPixels);
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
                playerProgressPixels[i] = new Color32(255, 255, 255, 255);
            }
        }
        _inGameplayHighlight.SetPixels32(playerProgressPixels);
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
        
        ImageManager.RemoveFocusColor(jar.ColorImage.color);
    }
    
    
}
