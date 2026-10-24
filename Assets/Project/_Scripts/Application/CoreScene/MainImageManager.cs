using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainImageManager : MonoBehaviour
{
    public SpriteRenderer mainImage;

    public Sprite mainImageSprite;
    public ColorJar ColorJarPrefab;
    public Transform ColorJarParent;

    private List<ColorJar> colorBars;

    public void Initialize()
    {
        mainImageSprite = mainImage.sprite;
        mainImage.sprite = DeskRenderer.GetLevelRender(mainImage.sprite.texture);
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
            
            ColorJar newColorBar = Instantiate(ColorJarPrefab, ColorJarParent);
            newColorBar.ColorImage.color = i.Key;
            newColorBar.SliderImage.color = i.Key;
            newColorBar.TextCount.text = i.Value.ToString();
            newColorBar.Count = i.Value;
            //newColorBar.Instantiate();
            colorBars.Add(newColorBar);
        }
        
        //colorBars.Shuffle();
    }
}