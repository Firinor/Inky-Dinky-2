using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
#if IS_YANDEX
using YG;
#endif

public class BOOTSTRAP : MonoBehaviour
{
    [SerializeField]
    private SceneButton nextScene;
    [SerializeField]
    private Settings settings;
    [SerializeField] 
    private AudioSource music;
    
    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
#if IS_YANDEX
        yield return YG2.onGetSDKData;
#endif
        
        settings.Initialize(bootstrap: true);
        
#if IS_GAMEMONETIZE
        GameMonetize.Instance.Init();
#endif
#if IS_MIRRA
        MirraService.Instance.Initialize();
#endif

        if(music != null)
            music.Play();
        nextScene.SwitchToScene();
    }
}