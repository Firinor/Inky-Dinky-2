using UnityEngine;
using UnityEngine.UI;

public class SetRandomBackground : MonoBehaviour
{
    [SerializeField] 
    private Sprite[] backgrounds;
    [SerializeField] 
    private SpriteRenderer backgroundRenderer;
    [SerializeField] 
    private Image backgroundImage;
    
    void Start()
    {
        Sprite random = backgrounds[Random.Range(0, backgrounds.Length)];
        if (backgroundRenderer != null)
            backgroundRenderer.sprite = random;
        if (backgroundImage != null)
            backgroundImage.sprite = random;
    }
}
