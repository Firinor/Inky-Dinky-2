using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HelperManager : MonoBehaviour
{
    public GameObject ChooseAssistance;

    public HelperCandidatе[] Candidates;

    public Sprite AddHelperIcon;
    public Sprite DefaultHelperIcon;
    
    public List<HelperButton> helpers;
    
    public void Initialize()
    {
        foreach (var helperButton in helpers)
        {
            helperButton.enabled = false;
            helperButton.AwatarImage.sprite = AddHelperIcon;
            helperButton.NameText.gameObject.SetActive(false);
            helperButton.Button.onClick.RemoveAllListeners();
            helperButton.Button.onClick.AddListener(WantAddHelper);
            if(helperButton != helpers[0])
                helperButton.gameObject.SetActive(false);
        }
        foreach (var Candidate in Candidates)
        {
            Candidate.Initialize();
        }
    }
    
    private void WantAddHelper()
    {
        ChooseAssistance.SetActive(true);

        //GetCandidats();
    }
    
    public void AddThisHelper(Sprite avatarSprite, string nameTextText, Sprite penSprite)
    {
        HelperButton lastHelper = helpers.FirstOrDefault(h => h.AwatarImage.sprite = AddHelperIcon);
        if(lastHelper is null)
            return;
        
        lastHelper.AwatarImage.sprite = avatarSprite;
        lastHelper.NameText.gameObject.SetActive(true);
        lastHelper.NameText.text = nameTextText;
        lastHelper.Button.onClick.RemoveAllListeners();
        //lastHelper.Button.onClick.AddListener(WantFireHelper);
        lastHelper.enabled = true;
        
    }
}