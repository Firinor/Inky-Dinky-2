using System.Collections;
using FirAnimations;
using UnityEngine;

public class CoreBootstrap : MonoBehaviour
{
    public FirAnimation closeСurtain; 
    
    [SerializeField] 
    private Settings settings;
    [SerializeField] 
    private SpellManager spells;
    [SerializeField] 
    private CameraMover cameraMover;
    [SerializeField] 
    private MainImageManager mainImage;
    [SerializeField] 
    private PlayerHand playerHand;
    
    private SaveData player;
    
    private IEnumerator Start()
    {
        //closeСurtain.Initialize();
        
        yield return null;
        
        //closeСurtain.Play();//OpenScene
        
        LoadPlayerData();
        
        //settings.Initialize();
        //spells.Initialize(player);
        cameraMover.Initialize();
        mainImage.Initialize();
        playerHand.Initialize();
    }

    private void LoadPlayerData()
    {
        player = SaveData.GetPlayer();
        player.FirstLoad();
    }
}
