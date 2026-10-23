using System.Collections.Generic;
using YG;

namespace YG
{
    public partial class SavesYG
    {
        public int GoldCoins;
        public string tilesID = "ClassicTiles";
        public string deskID = "ClassicDesk";
        public int Difficulty = 1;
#if IS_Collecting
        public int GameMode = 1;
#else
        public int GameMode;
#endif
        public int GoldMedals;
        public int SilverMedals;
        public int BronzeMedals;
        public List<LevelStars> LevelStars = new();
    }
}

public class YGSaveData : SaveData
{
    private SavesYG saves;

    public override int GoldCoins
    {
        get => saves.GoldCoins;
        set => saves.GoldCoins = value;
    }

    public override string TilesID
    {
        get => saves.tilesID;
        set => saves.tilesID = value;
    }

    public override string DeskID 
    {
        get => saves.deskID;
        set => saves.deskID = value;
    }
    public override int Difficulty 
    {
        get => saves.Difficulty;
        set => saves.Difficulty = value;
    }
    public override GameMode GameMode 
    {
        get => (GameMode)saves.GameMode;
        set => saves.GameMode = (int)value;
    }
    public override int GoldMedals
    {
        get => saves.GoldMedals;
        set => saves.GoldMedals = value;
    }
    public override int SilverMedals
    {
        get => saves.SilverMedals;
        set => saves.SilverMedals = value;
    }
    public override int BronzeMedals
    {
        get => saves.BronzeMedals;
        set => saves.BronzeMedals = value;
    }
    public override List<LevelStars> LevelStars
    {
        get => saves.LevelStars;
        set => saves.LevelStars = value;
    }
    
    public override void FirstLoad()
    {
        //saves = YG2.saves;
    }

    public override void AddGold(int count)
    {
        GoldCoins += count;
        InvokeGoldChange(GoldCoins);
    }
    public override void AddMedals(string fromDeskID, int count)
    {
        foreach (LevelStars levelStar in LevelStars)
        {
            if(!string.Equals(fromDeskID, levelStar.ID))
                continue;
            
            if(count <= levelStar.medalCount)
                return;

            levelStar.medalCount = count;
            return;
        }

        LevelStars level = new LevelStars()
        {
            ID = fromDeskID,
            medalCount = count
        };
        
        LevelStars.Add(level);
    }
    public override bool TrySpendGold(int count)
    {
        if (GoldCoins < count)
            return false;

        GoldCoins -= count;
        InvokeGoldChange(GoldCoins);
        return true;
    }

    public override void Save()
    {
        //YG2.SaveProgress();
    }
    
    public override void ResetProgress()
    {
        GoldCoins = 0;
        TilesID = "ClassicTiles";
        DeskID = "ClassicDesk";
        Difficulty = 1;
#if IS_Collecting
        GameMode = GameMode.Collecting;
#else
        GameMode = GameMode.Solitare;
#endif
        GoldMedals = 0;
        SilverMedals = 0;
        BronzeMedals = 0;
        LevelStars = new();
        InvokeGoldChange(GoldCoins);
        Save();
    }
}