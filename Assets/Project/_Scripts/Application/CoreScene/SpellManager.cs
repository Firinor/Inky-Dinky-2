using FirAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if IS_YANDEX
using YG;
#endif

public class SpellManager : MonoBehaviour
{
    [SerializeField] private Button spellShuffle;
    [SerializeField] private Button spellHint;
    [SerializeField] private Image ADSHint;
    [SerializeField] private Button spellSpotlight;
    [SerializeField] private CoreBootstrap bootstrap;
    [SerializeField] private FirAnimationsManager animations;
    
    [SerializeField] private Button spellShuffle2;
    [SerializeField] private Image ADSShuffle;
    
    [SerializeField] private GameObject SpellSupply;

    [SerializeField] private TextMeshProUGUI ShuffleCountText;
    [SerializeField] private TextMeshProUGUI HintCountText;
    [SerializeField] private TextMeshProUGUI SpotLightCountText;

    private SaveData player;

    private bool isAnimationComplete;
    private bool isSpotLightOn = false;
    
    public void Initialize(SaveData progress)
    {
        player = progress;
        
        spellShuffle.onClick.AddListener(Shuffle);
        if (player.GameMode == GameMode.Solitare)
        {
            if(player.Difficulty >= 3)
                spellShuffle2.onClick.AddListener(Shuffle);
            else
                spellShuffle2.onClick.AddListener(AdsShuffle);
        }
        spellShuffle.gameObject.SetActive(false);
        ADSShuffle.gameObject.SetActive(player.Difficulty < 3);
        
        spellSpotlight.gameObject.SetActive(false);

        if (progress.GameMode == GameMode.Solitare)
        {
            if(player.Difficulty >= 3)
                spellHint.onClick.AddListener(AdsHint);
            else
                spellHint.onClick.AddListener(Hint);
        }
        spellHint.gameObject.SetActive(false);
        ADSHint.gameObject.SetActive(player.Difficulty >= 3);
    }

    public void ButtonsOn()
    {
        spellShuffle.gameObject.SetActive(true);
        spellShuffle.GetComponent<ButtonClick>().Initialize();
        spellSpotlight.gameObject.SetActive(true);
        spellSpotlight.GetComponent<ButtonClick>().Initialize();
        if (player.GameMode == GameMode.Solitare)
        {
            spellHint.gameObject.SetActive(true);
            spellHint.GetComponent<ButtonClick>().Initialize();
        }
            
        if(isAnimationComplete)
            return;
        
        animations.Initialize();
        animations.StartAnimations();
        isAnimationComplete = true;
    }
    
    private string GetSpellCount(int count)
    {
        return count.ToString();
        //return count > 0 ? count.ToString() : "<color=green>+</color>";
    }

    private void AdsShuffle()
    {
#if IS_YANDEX
        YG2.RewardedAdvShow("Shuffle", Shuffle);
#else
        Shuffle();
#endif
    }

    public void DisableSpotLight()
    {
        isSpotLightOn = false;
    }
    private void Shuffle()
    {
        
    }

    private void AdsHint()
    {
#if IS_YANDEX
        YG2.RewardedAdvShow("Hint", Hint);
#else
        Hint();
#endif
    }

    private void Hint()
    {
        
    }
    private void TrySpotlight()
    {
        if (isSpotLightOn) Spotlight();
    }
    
    private void Spotlight()
    {

    }
    
    private void OnDestroy()
    {
        spellShuffle.onClick.RemoveAllListeners();
        spellShuffle2.onClick.RemoveListener(AdsShuffle);
        spellHint.onClick.RemoveAllListeners();
        spellSpotlight.onClick.RemoveAllListeners();
    }
}
