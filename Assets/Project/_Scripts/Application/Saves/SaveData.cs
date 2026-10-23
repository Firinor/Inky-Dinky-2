using System;
using System.Collections.Generic;

[Serializable]
public abstract class SaveData
{
    public abstract int GoldCoins { get; set; }
    public abstract int GoldMedals { get; set; }
    public abstract int SilverMedals { get; set; }
    public abstract int BronzeMedals { get; set; }
    public abstract List<LevelStars> LevelStars { get; set; }
    public abstract string TilesID { get; set; }
    public abstract string DeskID { get; set; }
    public abstract int Difficulty { get; set; }
    public abstract GameMode GameMode { get; set; }
    
    public int MedalsCount
    {
        get {
            if(LevelStars is null
               || LevelStars.Count == 0)
                return 0;

            int result = 0;
            foreach (LevelStars level in LevelStars)
            {
                result += level.medalCount;
            }
            return result;
        }
    }
    public int MedalsCountByLevel(string levelID)
    {
        if(LevelStars is null
           || LevelStars.Count == 0)
            return 0;
        
        foreach (LevelStars level in LevelStars)
        {
            if (string.Equals(level.ID, levelID))
                return level.medalCount;
        }
        return 0;
    }
    
    public event Action<int> OnGoldChange;

    public abstract void FirstLoad();
    
    public abstract void AddGold(int count);
    public abstract void AddMedals(string deskID, int count);
    public abstract bool TrySpendGold(int count);
    public abstract void ResetProgress();
    public abstract void Save();
    
    protected void InvokeGoldChange(int gold)
    {
        OnGoldChange?.Invoke(gold);
    }

    public static SaveData GetPlayer()
    {
#if IS_YANDEX
        return new YGSaveData();
#elif IS_MIRRA
        return new MirraSaveData();
#else
        return new PrefsSaveData();
#endif
    }
}

[Serializable]
public enum GameMode
{
    Solitare,
    Collecting,
    Slide
}

[Serializable]
public class LevelStars
{
    public string ID;
    public int medalCount;
}