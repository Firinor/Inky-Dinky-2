using UnityEngine;
using UnityEngine.UI;

public class DisableImageToggleSettings : MonoBehaviour
{
    public Image image;

    public void SwitchImage(bool v)
    {
        image.enabled = !v;
    }
}
