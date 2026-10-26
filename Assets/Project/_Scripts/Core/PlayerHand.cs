using System.Linq;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public MainImageManager ImageManager;
    public Transform[] BrushColorPoints;
    public PlayerInputHolder PlayerInput;

    public Material MainImageMaterial;
    
    public bool TryAddColorToBrush(ColorJar jar)
    {
        Transform emptyPoint = BrushColorPoints.FirstOrDefault(p => p.childCount == 0);
        if (emptyPoint is null)
            return false;

        jar.transform.SetParent(emptyPoint);
        jar.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        jar.Button.onClick.RemoveAllListeners();
        jar.Button.onClick.AddListener(() => RemoveColorFromBrush(jar));
        
        return true;
    }

    private void RemoveColorFromBrush(ColorJar jar)
    {
        jar.transform.SetParent(ImageManager.ColorJarParent);
        jar.transform.SetAsFirstSibling();
        jar.Button.onClick.RemoveAllListeners();
        jar.Button.onClick.AddListener(() => TryAddColorToBrush(jar));
    }
    
    
}
