using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{
    public static T GetRandom<T>(this List<T> list)
    {
        int index = Random.Range(0, list.Count);
        return  list[index];
    }
    
    public static T PullRandom<T>(this List<T> list)
    {
        int index = Random.Range(0, list.Count);
        T result = list[index];
        list.Remove(result);
        return result;
    }
    
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    
    public static void ClearAll(this Transform transform, bool instant = false)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if(instant)
                Object.DestroyImmediate(transform.GetChild(i).gameObject);
            else
                Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
    
    public static bool TryFindIndex<T>(this T[,] array, T target, out int i, out int j)
        where T : class
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        for (i = 0; i < rows; i++)
        for (j = 0; j < cols; j++)
            if (ReferenceEquals(array[i, j], target))
                return true;

        i = j = -1;
        return false;
    }
    
    public static bool IsEqualColorCode(this Color color, Color32 code)
    {
        return color.a.Equals(1) && (int)(color.r * 8) == code.r && (int)(color.g * 8) == code.g && (int)(color.b * 8) == code.b;
    }
    
    public static Color ToBrightColor(this Color32 a)
    {
        Color result = new Color();
        float center = 1 / 16f;
        result.r = a.r / 8f + center;
        if (result.r >= 1) result.r = 1;
        result.g = a.g / 8f + center;
        if (result.g >= 1) result.g = 1;
        result.b = a.b / 8f + center;
        if (result.b >= 1) result.b = 1;
        result.a = 1;
        return result;
    }
    public static Color32 FromColorCode(this Color32 a)
    {
        Color32 result = new Color32();
        byte center = 16;
        int value = a.r * 32 + center;
        if (value >= 255) value = 255;
        result.r = (byte)(value);
        value = a.g * 32 + center;
        if (value >= 255) value = 255;
        result.g = (byte)(value);
        value = a.b * 32 + center;
        if (value >= 255) value = 255;
        result.b = (byte)(value);
        result.a = byte.MaxValue;
        return result;
    }
}