using System;
using UnityEngine.Serialization;

[Serializable]
public class SettingsData
{
    public float MusicValue = .2f;
    public bool IsMusicOn = true;
    public float SFXValue = .2f;
    public bool IsSFXOn = true;
    public bool isPlayerLanguage = false;
    public string Language = "en";
}