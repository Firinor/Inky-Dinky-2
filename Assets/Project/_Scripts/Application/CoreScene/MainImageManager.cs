using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainImageManager : MonoBehaviour
{
    public PlayerHand PlayerHand;
    
    public SpriteRenderer mainImage;

    public Sprite mainImageSprite;
    public ColorJar ColorJarPrefab;
    public Transform ColorJarParent;

    private Sprite _inGameplaySprite;
    public int ImageLength => mainPixels.Length;
    public int ImageWidth;
    public int ImageHeight;
    
    public Material MainImageMaterial;
    
    public List<ColorJar> colorBars;

    public Color32[] mainPixels;

    public void Initialize()
    {
        mainImageSprite = mainImage.sprite;
        _inGameplaySprite = GetLevelRender(mainImage.sprite.texture);
        mainImage.sprite = _inGameplaySprite;
        GenerateColorSequence();
    }
    
    private Sprite GetLevelRender(Texture2D source)
    {
        ImageWidth = source.width;
        ImageHeight = source.height;
        var result = new Texture2D(ImageWidth, ImageHeight, TextureFormat.RGBA32, false){
            filterMode = FilterMode.Point
        };
        mainPixels = source.GetPixels32();
        result.SetPixels32(mainPixels);
        result.Apply();
        
        var playerProgressPixels = new Color32[mainPixels.Length];
        for (int i = 0; i < playerProgressPixels.Length; i++)
        {
            Color c = mainPixels[i];
            
            float gray = c.r * 0.2126f + c.g * 0.7152f + c.b * 0.0722f;
            float contrast = .2f;
            gray = (gray - 0.5f) * contrast + 0.5f;
            
            playerProgressPixels[i] = new Color(gray, gray, gray, c.a);
        }

        result.SetPixels32(playerProgressPixels);
        result.Apply();

        int pixelsPerUnit = 100;
        return Sprite.Create(
            result,
            new Rect(0, 0, source.width, source.height),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit);
    }
    
    private void GenerateColorSequence()
    {
        ColorJarParent.ClearAll(instant: true);
        
        Texture2D tex = mainImageSprite.texture;
        Rect rect = mainImageSprite.textureRect;
        
        Color[] pixels = tex.GetPixels(
            (int)rect.x, (int)rect.y,
            (int)rect.width, (int)rect.height
        );

        colorBars = new List<ColorJar>();
        
        Dictionary<Color, int> counts = pixels
            .GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());
            
        foreach (var i in counts)
        {
            if(i.Key.a < 1)
                continue;
            
            ColorJar newColorJar = Instantiate(ColorJarPrefab, ColorJarParent);
            newColorJar.ColorImage.color = i.Key;
            newColorJar.SliderImage.color = i.Key;
            newColorJar.TextCount.text = i.Value.ToString();
            newColorJar.Count = i.Value;
            newColorJar.Button.onClick.AddListener(() => PlayerHand.TryAddColorToBrush(newColorJar));
            newColorJar.Initialize();
            colorBars.Add(newColorJar);
        }
        
        //colorBars.Shuffle();
    }

    public void AddFocusColor(Color colorImageColor)
    {
        
    }

    public void RemoveFocusColor(Color colorImageColor)
    {
        
    }
}


public class Pixel
{
    public Color32 ID;
    public Color32 GrayState;
    public int x;
    public int y;
    
}