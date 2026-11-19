using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelperCandidatе : MonoBehaviour
{
    public HelperManager manager;
    
    public Image Avatar;
    public TextMeshProUGUI NameText;
    public Image Pen;
    public Button Button;

    public void Initialize()
    {
        Button.onClick.AddListener(AddThisCandidate);
    }

    private void AddThisCandidate()
    {
        manager.AddThisHelper(Avatar.sprite, NameText.text, Pen.sprite);
    }

    private void OnDestroy()
    {
        Button.onClick.RemoveAllListeners();
    }
}