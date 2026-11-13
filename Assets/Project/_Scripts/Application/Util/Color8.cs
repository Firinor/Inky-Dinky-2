using System;
using UnityEngine;

public struct Color8 : IEquatable<Color8>
{
    public byte r;
    public byte g;
    public byte b;
    public float a;

    public Color8(Color32 color32)
    {
        r = (byte)(color32.r / 32);
        g = (byte)(color32.g / 32);
        b = (byte)(color32.b / 32);
        a = color32.a / (float)byte.MaxValue;
    }
    public Color8(Color color)
    {
        r = (byte)(color.r * 8);
        if (color.r >= 1) r = 7;
        g = (byte)(color.g * 8);
        if (color.g >= 1) g = 7;
        b = (byte)(color.b * 8);
        if (color.b >= 1) b = 7;
        a = color.a;
    }
    
    public Color GetColor()
    {
        float center = 1 / 16f;
        Color result = new Color();
        result.r = r / 8f + center;
        result.g = g / 8f + center;
        result.b = b / 8f + center;
        result.a = a;
        return result;
    }
    
    public bool IsEqualTo(Color other)
    {
        return r == (byte)(other.r * 8) && g == (byte)(other.g * 8) && b == (byte)(other.b * 8);
    }
    public bool IsEqualTo(Color32 other)
    {
        return r == (byte)(other.r / 32) && g == (byte)(other.g / 32) && b == (byte)(other.b / 32);
    }

    public bool Equals(Color8 other)
    {
        return r == other.r && g == other.g && b == other.b && a == other.a;
    }

    public override bool Equals(object obj)
    {
        return obj is Color8 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(r, g, b, a);
    }
}