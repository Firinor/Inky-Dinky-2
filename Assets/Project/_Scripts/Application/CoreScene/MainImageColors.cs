using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainImageColors : MonoBehaviour
{
    public Button ColorButtonPrefab;
    public Transform ColorButtonParent;
    public Image MainImage;
    public Sprite defaultSprite;
    public MainImageManager MainImageManager;
    
    
    [SerializeField] private float blinkDuration = 2f;
    [SerializeField] private float blinkInterval = 0.15f;

    private List<int> indices;
    private Texture2D tex;
    
    [ContextMenu(nameof(CheckColors))]
    public void CheckColors()
    {
        ColorButtonParent.ClearAll();
        defaultSprite = MainImage.sprite;
        Sprite mainSprite = MainImage.sprite;
        
        Texture2D tex = mainSprite.texture;
        Rect rect = mainSprite.textureRect;
        
        Color[] pixels = tex.GetPixels(
            (int)rect.x, (int)rect.y,
            (int)rect.width, (int)rect.height
        );
        
        Dictionary<Color, int> counts = pixels
            .GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());
        
        foreach (var i in counts)
        {
            if(i.Key.a < 1)
                continue;
            
            Button newColorBar = Instantiate(ColorButtonPrefab, ColorButtonParent);
            newColorBar.GetComponent<Image>().color = i.Key;
            newColorBar.GetComponentInChildren<TextMeshProUGUI>().text = i.Value + "/" + ColorUtility.ToHtmlStringRGB(i.Key);
            newColorBar.onClick.AddListener(() => ColorBlink(i.Key));
        }
    }

    private void ColorBlink(Color color)
    {
        StopAllCoroutines();
        MainImage.sprite = defaultSprite;
        
        Texture2D src = MainImage.sprite.texture;
        tex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(src.GetPixels());
        tex.Apply();
        Rect r = MainImage.sprite.textureRect;
        
        MainImage.sprite = Sprite.Create(
            tex,
            r,
            new Vector2(0.5f, 0.5f)
        );

        var originalPixels = tex.GetPixels();
        
        indices = new List<int>();
        for (int i = 0; i < originalPixels.Length; i++)
        {
            if (originalPixels[i] == color)
                indices.Add(i);
        }

        StartCoroutine(BlinkRoutine(color));
    }

    private IEnumerator BlinkRoutine(Color color)
    {
        float elapsed = 0f;
        int ColorIndex = 0;

        while (elapsed < blinkDuration)
        {
            Color32[] pixels = tex.GetPixels32();
            for (int i = 0; i < indices.Count; i++)
            {
                int idx = indices[i];
                pixels[idx] = ColorIndex switch
                {
                    0 => Color.white,
                    1 => Color.black,
                    _ => color
                };
            }
            tex.SetPixels32(pixels);
            tex.Apply();

            ColorIndex++;
            ColorIndex = ColorIndex%3;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        MainImage.sprite = defaultSprite;
    }
}