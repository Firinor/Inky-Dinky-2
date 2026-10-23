using UnityEngine;
using UnityEngine.UI;
#if IS_YANDEX
using YG;
#endif

public class ReviewYG : MonoBehaviour
{
    public Button reviewButton;
    void Start()
    {
#if IS_YANDEX
        YG2.onReviewSent += ReviewSent;
        
        if (YG2.reviewCanShow)
            reviewButton.onClick.AddListener(YG2.ReviewShow);
        else
            Destroy(gameObject);
#else
        Destroy(gameObject);        
#endif
    }

    private void ReviewSent(bool sent)
    {
        if (sent)
            Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        reviewButton.onClick.RemoveAllListeners();
#if IS_YANDEX
        YG2.onReviewSent -= ReviewSent;
#endif
    }
}
