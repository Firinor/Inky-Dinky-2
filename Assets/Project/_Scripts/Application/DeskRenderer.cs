using UnityEngine;

public static class DeskRenderer
{
    public static Sprite GetLevelRender(Texture2D source)
    {
        var result = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false){
            filterMode = FilterMode.Point
        };
        result.SetPixels(source.GetPixels());
        result.Apply();
        
        var pixels = result.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            Color c = pixels[i];
            
            float gray = c.r * 0.2126f + c.g * 0.7152f + c.b * 0.0722f;
            float contrast = .2f;
            gray = (gray - 0.5f) * contrast + 0.5f;
            
            pixels[i] = new Color(gray, gray, gray, c.a);
        }

        result.SetPixels(pixels);
        result.Apply();

        int pixelsPerUnit = 100;
        return Sprite.Create(
            result,
            new Rect(0, 0, source.width, source.height),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit);
    }
}