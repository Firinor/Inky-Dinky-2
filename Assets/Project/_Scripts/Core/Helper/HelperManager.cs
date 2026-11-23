using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HelperManager : MonoBehaviour
{
    public GameObject ChooseAssistance;
    public SpriteRenderer MainSprite;

    public HelperCandidatе[] Candidates;

    public Sprite AddHelperIcon;
    public Sprite DefaultHelperIcon;
    
    public List<HelperButton> helpers;
    
    public void Initialize()
    {
        foreach (HelperButton helperButton in helpers)
        {
            helperButton.enabled = false;
            helperButton.AwatarImage.sprite = AddHelperIcon;
            helperButton.NameText.gameObject.SetActive(false);
            helperButton.Button.onClick.RemoveAllListeners();
            helperButton.Button.onClick.AddListener(WantAddHelper);
            if(helperButton != helpers[0])
                helperButton.gameObject.SetActive(false);
        }
        HelperAssistant.StartImagePosition = MainSprite.sprite.bounds.min;
        
        foreach (var Candidate in Candidates)
        {
            Candidate.Initialize();
        }
        ChooseAssistance.SetActive(false);
    }
    
    private void WantAddHelper()
    {
        ChooseAssistance.SetActive(true);

        //GetCandidats();
    }
    
    public void AddThisHelper(Sprite avatarSprite, string nameTextText, Sprite penSprite)
    {
        HelperButton lastHelper = helpers.FirstOrDefault(h => h.AwatarImage.sprite == AddHelperIcon);
        if(lastHelper is null)
            return;
        
        lastHelper.AwatarImage.sprite = avatarSprite;
        lastHelper.NameText.gameObject.SetActive(true);
        lastHelper.NameText.text = nameTextText;
        lastHelper.Button.onClick.RemoveAllListeners();
        //lastHelper.Button.onClick.AddListener(WantFireHelper);
        lastHelper.enabled = true;

        lastHelper.DoEnableAssistant(penSprite);

        HelperButton nextHelper = helpers.FirstOrDefault(h => !h.gameObject.activeSelf);
        if(nextHelper is null)
            return;
        
        nextHelper.gameObject.SetActive(true);
        nextHelper.enabled = true;
        nextHelper.AwatarImage.sprite = AddHelperIcon;
        nextHelper.NameText.gameObject.SetActive(false);
        nextHelper.Button.onClick.RemoveAllListeners();
        nextHelper.Button.onClick.AddListener(WantAddHelper);
    }
}