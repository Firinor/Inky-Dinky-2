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
    
    private List<ColorJar> colorBars;

    public void Initialize()
    {
        mainImageSprite = mainImage.sprite;
        _inGameplaySprite = DeskRenderer.GetLevelRender(mainImage.sprite.texture);
        mainImage.sprite = _inGameplaySprite;
        GenerateColorSequence();
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
            //newColorBar.Instantiate();
            colorBars.Add(newColorJar);
        }
        
        //colorBars.Shuffle();
    }
}