using UnityEngine;

public class DoNotDestroyOnLoadMusic : MonoBehaviour
{
    private static DoNotDestroyOnLoadMusic instance;
    
    private void Awake()
    {
        if (instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}