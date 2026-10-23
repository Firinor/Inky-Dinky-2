using UnityEngine;
using UnityEngine.UI;

namespace FirYandexService
{
    public class ADSButtons : MonoBehaviour
    {
        [SerializeField] 
        private Button[] buttons;

#if IS_YANDEX
        private void Awake()
        {
            if(FirYG2Service.instance == null)
                return;
            FirYG2Service.instance.SetButtons(buttons);
        }
#elif IS_GAMEMONETIZE
        private void Awake()
        {
            if(GameMonetize.Instance == null)
                return;
            GameMonetize.Instance.SetButtons(buttons);
        }
#elif IS_MIRRA
        private void Awake()
        {
            if(MirraService.Instance == null)
                return;
            MirraService.Instance.SetButtons(buttons);
        }
#else
        private void Awake()
        {
            enabled = false;
            Destroy(gameObject);
        }
#endif
    }
}
