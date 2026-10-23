using FirAnimations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public string SceneName;

    public FirAnimation closeСurtain;    
    
    public void SwitchToScene()
    {
        if(closeСurtain == null)
            SceneManager.LoadScene(SceneName);
        else
        {
            closeСurtain.OnComplete = null;
            closeСurtain.OnComplete = () => { SceneManager.LoadScene(SceneName);};
            closeСurtain.Play();
        }
    }
}
