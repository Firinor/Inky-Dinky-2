using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Audio")]
public class AudioConfig : ScriptableObject
{
    [Header("Buttons")] 
    public ClipSettings ButtonClick;
    public ClipSettings OpenScroll;
    public ClipSettings CloseScroll;
    public ClipSettings HelpSpell;
    [Header("Tiles")] 
    public ClipSettings StartCollide;
    public ClipSettings EndCollide;
    public ClipSettings TileSelect;
    public ClipSettings TileError;
    [Header("Win")] 
    public ClipSettings Win;
    public ClipSettings Lose;
    public ClipSettings Level;
    public ClipSettings Unlock;
    public ClipSettings Scores;
}