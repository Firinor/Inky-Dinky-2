using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
#if IS_YANDEX
using YG;
#endif
#if IS_MIRRA
using MirraGames.SDK;
using MirraGames.SDK.Common;
#endif

public class Settings : MonoBehaviour
{
    public SettingsData data;

    [SerializeField] private AudioMixer mixer;
    
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle sfxToggle;
    
    [SerializeField] private Toggle enLanguageToggle;
    [SerializeField] private Toggle ruLanguageToggle;
    
    public void Initialize(bool bootstrap = false)
    {
        data = SaveLoadSystem<SettingsData>.Load("Settings", new ());

        if (bootstrap)
        {
            SetMixerValues();
        }
        else
        {
            Subscribe();
            SetSounds();
            StartCoroutine(SetLanguage());
        }
    }

    private void SetMixerValues()
    {
        if(data.IsMusicOn)
            mixer.SetFloat("MusicVolume", Mathf.Log10(data.MusicValue) * 20);
        else
            mixer.SetFloat("MusicVolume", -80);
        
        if(data.IsSFXOn)
            mixer.SetFloat("SFXVolume", Mathf.Log10(data.SFXValue) * 20);
        else
            mixer.SetFloat("SFXVolume", -80);
        
        
        Locale locale;
        if (data.isPlayerLanguage)
        {
            if(data.Language.Equals("ru-RU"))
                locale = LocalizationSettings.AvailableLocales.GetLocale("ru-RU");
            else
                locale = LocalizationSettings.AvailableLocales.GetLocale("en");
        }
        else
        {
#if IS_YANDEX
            if(string.Equals(YG2.lang, "ru"))
                locale = LocalizationSettings.AvailableLocales.GetLocale("ru-RU");
            else
                locale = LocalizationSettings.AvailableLocales.GetLocale("en");
#elif IS_MIRRA
            if(MirraSDK.Language.Current == LanguageType.Russian)
                locale = LocalizationSettings.AvailableLocales.GetLocale("ru-RU");
            else
                locale = LocalizationSettings.AvailableLocales.GetLocale("en");
#else
            locale = LocalizationSettings.AvailableLocales.GetLocale("en");
#endif
        }
        
        LocalizationSettings.SelectedLocale = locale;
    }

    private void Subscribe()
    {
        musicToggle.onValueChanged.AddListener(v =>
        {
            if(!v)
                mixer.SetFloat("MusicVolume", Mathf.Log10(data.MusicValue) * 20);
            else
                mixer.SetFloat("MusicVolume", -80);

            //musicToggle.GetComponent<Image>().sprite = v ? MusicOnSprite : MusicOffSprite;
            data.IsMusicOn = !v;
            SaveLoadSystem<SettingsData>.Save("Settings", data);
        });
        musicSlider.onValueChanged.AddListener(value =>
        {
            if(data.IsMusicOn)
                mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
            else
                mixer.SetFloat("MusicVolume", -80);

            data.MusicValue = value;
            SaveLoadSystem<SettingsData>.Save("Settings", data);
        });
        sfxToggle.onValueChanged.AddListener(v =>
        {
            if(!v)
                mixer.SetFloat("SFXVolume", Mathf.Log10(data.SFXValue) * 20);
            else
                mixer.SetFloat("SFXVolume", -80);
            
            //sfxToggle.GetComponent<Image>().sprite = v ? SoundOnSprite : SoundOffSprite;
            data.IsSFXOn = !v;
            SaveLoadSystem<SettingsData>.Save("Settings", data);
        });
        sfxSlider.onValueChanged.AddListener(value =>
        {
            if(data.IsSFXOn)
                mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
            else
                mixer.SetFloat("SFXVolume", -80);

            data.SFXValue = value;
            SaveLoadSystem<SettingsData>.Save("Settings", data);
        });
        
        ruLanguageToggle.onValueChanged.AddListener(v =>
        {
            if(!v) return;
                
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale("ru-RU");
            LocalizationSettings.SelectedLocale = locale;

            data.Language = "ru-RU";
            data.isPlayerLanguage = true;
            SaveLoadSystem<SettingsData>.Save("Settings", data);
        });
        
        enLanguageToggle.onValueChanged.AddListener(v =>
        {
            if(!v) return;
                
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale("en");
            LocalizationSettings.SelectedLocale = locale;
            
            data.Language = "en";
            data.isPlayerLanguage = true;
            SaveLoadSystem<SettingsData>.Save("Settings", data);
        });
    }

    private IEnumerator SetLanguage()
    {
        yield return LocalizationSettings.InitializationOperation;

        if (data.isPlayerLanguage)
        {
            if (data.Language == "ru-RU")
            {
                ruLanguageToggle.GetComponent<ButtonClick>().enabled = false;
                ruLanguageToggle.isOn = true;
                ruLanguageToggle.GetComponent<ButtonClick>().enabled = true;
            }
            else
            {
                enLanguageToggle.GetComponent<ButtonClick>().enabled = false;
                enLanguageToggle.isOn = true;
                enLanguageToggle.GetComponent<ButtonClick>().enabled = true;
            }
        }
        else
        {
#if IS_YANDEX
            if(string.Equals(YG2.lang, "ru"))
            {
                ruLanguageToggle.GetComponent<ButtonClick>().enabled = false;
                ruLanguageToggle.isOn = true;
                ruLanguageToggle.GetComponent<ButtonClick>().enabled = true;
            }
            else
            {
                enLanguageToggle.GetComponent<ButtonClick>().enabled = false;
                enLanguageToggle.isOn = true;
                enLanguageToggle.GetComponent<ButtonClick>().enabled = true;
            }
#endif
        }
    }

    private void SetSounds()
    {
        musicToggle.GetComponent<ButtonClick>().enabled = false;
        sfxToggle.GetComponent<ButtonClick>().enabled = false;
        
        musicSlider.value = data.MusicValue;
        musicToggle.isOn = !data.IsMusicOn;
        //musicToggle.GetComponent<Image>().sprite = data.IsMusicOn ? MusicOnSprite : MusicOffSprite;
        sfxSlider.value = data.SFXValue;
        sfxToggle.isOn = !data.IsSFXOn;
        //sfxToggle.GetComponent<Image>().sprite = data.IsSFXOn ? SoundOnSprite : SoundOffSprite;
        
        musicToggle.GetComponent<ButtonClick>().enabled = true;
        sfxToggle.GetComponent<ButtonClick>().enabled = true;
    }
    
    private void OnDestroy()
    {
        musicToggle?.onValueChanged.RemoveAllListeners();
        musicSlider?.onValueChanged.RemoveAllListeners();
        sfxToggle?.onValueChanged.RemoveAllListeners();
        sfxSlider?.onValueChanged.RemoveAllListeners();
        
        ruLanguageToggle?.onValueChanged.RemoveAllListeners();
        enLanguageToggle?.onValueChanged.RemoveAllListeners();
    }
}