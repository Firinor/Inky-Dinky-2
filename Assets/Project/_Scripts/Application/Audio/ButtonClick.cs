using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    private Toggle toggle;
    private Button button;
    public ESound sound = ESound.Click;
    
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.3f);
        toggle = GetComponent<Toggle>();

        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(v => OnClickSound());
            yield break;
        }

        if (button is null)
            button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnClickSound);
        }
    }

    public void Initialize()
    {
        if (button is null)
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClickSound);
        }
    }
    
    public void OnClickSound()
    {
        if(!enabled)
            return;
        
        if(SoundManager.Instance == null)
            return;
        
        //Debug.Log(name);
        if(sound == ESound.Click)
        {
            SoundManager.Instance.PlayButtonClick();
        }
        else if(sound == ESound.OpenScroll)
        {
            SoundManager.Instance.PlayOpenScroll();
        }
        else if(sound == ESound.CloseScroll)
        {
            SoundManager.Instance.PlayCloseScroll();
        }
        else if(sound == ESound.Help)
        {
            SoundManager.Instance.PlayHelpSpell();
        }
    }

    private void OnDestroy()
    {
        toggle?.onValueChanged.RemoveAllListeners();
        button?.onClick.RemoveAllListeners();
    }
}

public enum ESound
{
    Click,
    OpenScroll,
    CloseScroll,
    Help
}