using System;
using UnityEngine;
using UnityEngine.UI;
#if IS_MIRRA
using MirraGames.SDK;
#endif

public class MirraService : MonoBehaviour
{
    [SerializeField] private float Timer;
    [SerializeField] private float timeLeft;
    
#if IS_MIRRA
    public static MirraService Instance;
    
    private Button[] buttons;

    public static Action OnResumeGame;
    public static Action OnPauseGame;

    public void Awake()
    {
        if(Instance != null)
            Debug.LogError("Duo instance!");
        Instance = this;

        DontDestroyOnLoad(this);
    }

    public void Initialize()
    {
#if UNITY_WEBGL
        try
        {
            MirraSDK.WaitForProviders(() =>
            {
                timeLeft = Timer;
                MirraSDK.Analytics.GameIsReady();
                Debug.Log("[ADMobService] MirraSDK providers ready");
            });
        }
        catch (Exception e)
        {
            Debug.LogError("[AdMobService] Failed to show interstitial: " + e.Message);
        }
#endif
    }
    
    private void Update()
    {
        timeLeft -= Time.deltaTime;
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
    
    public void CheckTimerAd()
    {
        if (timeLeft > 0) 
            return;
        
        ShowAd();
    }
    
    internal void ShowAd()
    {
        MirraSDK.Ads.InvokeInterstitial(
            onOpen: () => Debug.Log("Межстраничная реклама открыта"),
            onClose: (isSuccess) =>
            {
                timeLeft = Timer;
                Debug.Log("Межстраничная реклама закрыта");
            });
    }
    
    /// <summary>
    /// Resume the game, this method is used when an ad has been showed
    /// </summary>
    void ResumeGame()
    {
        AudioListener.volume = 1f;
        Time.timeScale = 1f;
        
        if (OnResumeGame != null) OnResumeGame();
    }

    /// <summary>
    /// Pause the game, this method is used when we show an ad
    /// </summary>
    void PauseGame()
    {
        Time.timeScale = Mathf.Epsilon;
        AudioListener.volume = 0f;
        
        if(OnPauseGame != null) OnPauseGame();
    }
    
#else
    private void Awake()
    {
        enabled = false;
        Destroy(gameObject);
    }
#endif
}
