using System;
using UnityEngine;

public class HelperAssistant : MonoBehaviour
{
    public Transform Owner;
    public ColorJar jar;
    public float correctColorDelay;
    public float speed;
    
    public MainImageManager ImageManager;
    public PlayerHand PlayerHand;
    
    private Color32 color32;
    private Vector3 targetPosition;
    private float timer;
    
    public void GetNextColorJar()
    {
        jar = ImageManager.GetAnyColorJar();
        ImageManager.RemoveColorJar(jar);
        
        jar.transform.SetParent(Owner);
        jar.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        jar.Button.onClick.RemoveAllListeners();
        jar.Button.onClick.AddListener(RemoveColorJar);

        color32 = jar.codeColor;
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
