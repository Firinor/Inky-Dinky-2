using System;
using System.Collections.Generic;

[Serializable]
public class PrefsSaveData : SaveData
{
    public int goldCoins;
    public string tilesID = "ClassicTiles";
    public string deskID = "ClassicDesk";
    public int difficulty = 1;
    public GameMode gameMode;
    public int goldMedals;
    public int silverMedals;
    public int bronzeMedals;
    public List<LevelStars> levelStars = new();
    
    public override int GoldCoins
    {
        get => goldCoins;
        set => goldCoins = value;
    }
    public override string TilesID 
    {
        get => tilesID;
        set => tilesID = value;
    }
    public override string DeskID 
    {
        get => deskID;
        set => deskID = value;
    }
    public override int Difficulty 
    {
        get => difficulty;
        set => difficulty = value;
    }

    public override GameMode GameMode
    {
        get => gameMode;
        set => gameMode = value;
    }
    public override int GoldMedals
    {
        get => goldMedals;
        set => goldMedals = value;
    }
    public override int SilverMedals
    {
        get => silverMedals;
        set => silverMedals = value;
    }
    public override int BronzeMedals
    {
        get => bronzeMedals;
        set => bronzeMedals = value;
    }
    public override List<LevelStars> LevelStars
    {
        get => levelStars;
        set => levelStars = value;
    }
    public override void FirstLoad()
    {
        var data = SaveLoadSystem<PrefsSaveData>.Load("Player", new ());
        goldCoins = data.GoldCoins;
        tilesID = data.tilesID;
        deskID = data.deskID;
        difficulty = data.difficulty;
        gameMode = data.gameMode;
        goldMedals = data.goldMedals;
        silverMedals = data.silverMedals;
        bronzeMedals = data.bronzeMedals;
        levelStars = data.levelStars;
    }

    public override void AddGold(int count)
    {
        GoldCoins += count;
        InvokeGoldChange(GoldCoins);
    }

    public override void AddMedals(string fromDeskID, int count)
    {
        foreach (LevelStars levelStar in levelStars)
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
        
        levelStars.Add(level);
    }

    public override bool TrySpendGold(int count)
    {
        if (GoldCoins < count)
            return false;

        GoldCoins -= count;
        InvokeGoldChange(GoldCoins);
        return true;
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
        levelStars = new();
        Save();
    }

    public override void Save()
    {
        SaveLoadSystem<PrefsSaveData>.Save("Player", this);
    }
}