using System;
using UnityEngine;
using UnityEngine.UI;
#if IS_YANDEX
using YG;
using YG.Insides;
#endif

public class FirYG2Service : MonoBehaviour
{
    public static FirYG2Service instance;
    
    [SerializeField] private float Timer;
    
    private Coroutine timerAdShowCoroutine;
    private Button[] buttons;
    
#if IS_YANDEX
    private void Awake()
    {
        if(instance != null)
            Debug.LogError("Duo instance!");
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        YG2.onPurchaseSuccess += SuccessPurchased;
        YG2.onAdvNotification += PauseGame;
        YG2.onCloseAnyAdv  += UnpauseGame;
    }
    
    private void PauseGame() => YG2.PauseGame(true);
    private void UnpauseGame() => YG2.PauseGame(false);
    public void Initialize()
    {
        YG2.GameReadyAPI();
    }

    public void SetButtons(Button[] _buttons)
    {
        if (buttons != null)
        {
            foreach (var button in buttons)
            {
                if(button != null)
                    button.onClick.RemoveListener(CheckTimerAd);
            }
        }

        buttons = _buttons;
        foreach (var button in buttons)
        {
            button.onClick.AddListener(CheckTimerAd);
        }
    }
    
    private void Update()
    {
        Timer = YG2.timerInterAdv;
    }
    
    /// <summary>
    /// InterstitialAdv
    /// </summary>
    public void CheckTimerAd()
    {
        if (!AdReady()) 
            return;

        YG2.InterstitialAdvShow();
    }

    public bool AdReady()
    {
        return YG2.isTimerAdvCompleted && !YG2.nowAdsShow;
    }

    [ContextMenu("Pause")]
    void Pause()
    {
        YG2.PauseGame(!YG2.isPauseGame);   
    }
    
    [ContextMenu("ResetAdsTimer")]
    void ResetAdsTimer()
    {
        YGInsides.ResetTimerInterAdv();
    }

    /// <summary>
    /// RewardedAdv
    /// </summary>
    public void ShowRewarded()
    {
        Pause();
        YG2.RewardedAdvShow("randomBonus", RewardedBonus);
    }

    private void RewardedBonus()
    {
        YG2.SkipNextInterAdCall();
    }
    
    /// <summary>
    /// Purchase
    /// </summary>
    private void SuccessPurchased(string ID)
    {
        switch (ID)
        {
            case "NoAds":
            {
                
                break;
            }
            default:
                throw new Exception("Unknown Purchase ID!");
        }
    }

    private void OnDestroy()
    {
        YG2.onPurchaseSuccess -= SuccessPurchased;
        YG2.onAdvNotification -= PauseGame;
        YG2.onCloseAnyAdv  -= UnpauseGame;
    }
#else
    private void Awake()
    {
        enabled = false;
        Destroy(gameObject);
    }
#endif
}