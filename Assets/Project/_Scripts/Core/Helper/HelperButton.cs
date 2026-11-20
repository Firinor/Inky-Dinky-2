using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelperButton : MonoBehaviour
{
    public Image AwatarImage;
    public TextMeshProUGUI NameText;
    public Button Button;
    
    
    public MainImageManager ImageManager;
    public PlayerHand PlayerHand;
    public ColorJar jar;
    public Transform ColorJarPoint;
    public HelperAssistant HelperPen;

    public void OnEnable()
    {
        //GetNextColorJar();
        //HelperPen.gameObject.SetActive(true);
    }
    
    public void GetNextColorJar()
    {
        jar = ImageManager.GetAnyColorJar();
        ImageManager.RemoveColorJar(jar);
        
        jar.transform.SetParent(ColorJarPoint);
        jar.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        jar.Button.onClick.RemoveAllListeners();
        jar.Button.onClick.AddListener(RemoveColorJar);

        HelperPen.color32 = jar.codeColor;
        gameObject.SetActive(true);
    }

    private void RemoveColorJar()
    {
        jar.Button.onClick.RemoveAllListeners();
        jar.Button.onClick.AddListener(() => PlayerHand.TryAddColorToBrush(jar));
        if (jar.Count > 0)
        {
            jar.transform.SetParent(ImageManager.ColorJarParent);
            jar.transform.SetAsFirstSibling();
        }
        else
        {
            jar.transform.SetParent(null);
            Destroy(jar.gameObject);
        }
        
        if (PlayerHand.IsAuto)
        {
            GetNextColorJar();
        }
    }
}
