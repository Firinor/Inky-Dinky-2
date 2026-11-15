using UnityEngine;
using UnityEngine.UI;
#if IS_YANDEX
using YG;
#endif

public class SpellManager : MonoBehaviour
{
    [SerializeField] private Button spellShuffle;
    [SerializeField] private Button spellHint;
    [SerializeField] private Button spellSpotlight;
    [SerializeField] private CoreBootstrap bootstrap;
    
    private SaveData player;
    
    public void Initialize(SaveData progress)
    {
        player = progress;
        
    }
}
