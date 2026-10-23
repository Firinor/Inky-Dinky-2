using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlyLootManager : MonoBehaviour
{
    public static FlyLootManager instance;

    [SerializeField] 
    private Transform parent;
    [SerializeField] 
    private FlyLoot prefab;
    [SerializeField] 
    private float spawnRadius;
    [SerializeField] 
    private float spawnTotalTime = 1f;

    [Header("Test")]
    [SerializeField] 
    private Sprite testSprite;
    [SerializeField] 
    private Transform testStartpoint;
    [SerializeField] 
    private Transform testEndpoint;
    [SerializeField] 
    private int testCount;

    private List<FlyLoot> lootPool = new();
    
    private void Awake()
    {
        instance = this;
    }

    public void AnimateGoods(Sprite goods, Transform startPoint, Transform endPoint, int count = 1)
    {
        StartCoroutine(AnimateGoodsCoroutine(goods, startPoint, endPoint, count));
    }

    private IEnumerator AnimateGoodsCoroutine(Sprite goods, Transform startPoint, Transform endPoint, int count = 1)
    {
        bool isOffset = count > 1;
        
        float timer = 0;
        float yieldDelay = spawnTotalTime/count;

        for (int i = 0; i < count; i++)
        {
            timer -= yieldDelay;
            while (timer < 0)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            
            FlyLoot newGoods = Instantiate(prefab, startPoint.position , Quaternion.identity, parent);
            lootPool.Add(newGoods);
            if (isOffset)
            {
                newGoods.SetDestination(goods, endPoint, randomOffset);
            }
            else
                newGoods.SetDestination(goods, endPoint);

            newGoods.OnPointerEnterAction += FlyToEnd;
        }
    }

    public void FlyToEnd()
    {
        foreach (FlyLoot loot in lootPool)
        {
            loot.FlyToEnd();
            loot.OnPointerEnterAction -= FlyToEnd;
        }
    }
    
    [ContextMenu(nameof(Test))]
    private void Test()
    {
        StartCoroutine(AnimateGoodsCoroutine(testSprite, testStartpoint, testEndpoint, testCount));
    }
}