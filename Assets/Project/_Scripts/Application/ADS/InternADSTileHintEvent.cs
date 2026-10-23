using UnityEngine;
using UnityEngine.Serialization;

namespace FirYandexService
{
    public class InternADSTileHintEvent : MonoBehaviour
    {
#pragma warning disable CS0414
        [SerializeField] 
        private float timer = 60f;
        private float _timer;
#pragma warning restore CS0414
        
        private void Awake()
        {
#if IS_YANDEX
            if(FirYG2Service.instance == null)
            {
                SelfDestroy();
                return;
            }
#elif IS_GAMEMONETIZE
            if(GameMonetize.Instance == null)
            {
                SelfDestroy();
                return;
            }
#elif IS_MIRRA
            if(MirraService.Instance == null)
            {
                SelfDestroy();
                return;
            }
#endif
            _timer = timer;
            //rulesManager.OnTilesChanged += CheckTimer;
        }

        private void CheckTimer()
        {
            if(_timer > 0) 
                return;
#if IS_YANDEX
            if(!FirYG2Service.instance.AdReady())
                return;
            FirYG2Service.instance.CheckTimerAd();
#elif IS_GAMEMONETIZE
            GameMonetize.Instance.ShowAd();
#elif IS_MIRRA
            MirraService.Instance.ShowAd();
#endif         
            _timer = timer;
        }
        
        private void Update()
        {
            _timer -= Time.deltaTime;
        }

        private void OnDestroy()
        {
            if(FirYG2Service.instance == null)
                return;
            //rulesManager.OnTilesChanged -= CheckTimer;
        }
        
        private void SelfDestroy()
        {
            enabled = false;
            Destroy(gameObject);
        }
    }
}