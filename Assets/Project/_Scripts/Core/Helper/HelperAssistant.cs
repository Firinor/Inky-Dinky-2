using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelperAssistant : MonoBehaviour
{
    public event Action<Vector2Int> OnColorDraw;
    public event Action OnColorJarEnd;

    [Range(0, 1)]
    public float idleChance = 0.005f;
    
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float minSpeed = 1.5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float slowDownRadius = 2f;
    
    public SpriteRenderer Pen;
    public SpriteRenderer Avatar;
    
    public List<Vector2Int> map;
    public List<List<Vector2Int>> chanks;
    public Vector2Int pointToRemove;
        
    private Vector3 targetPosition;
    private float timer;

    public static Vector3 StartImagePosition;

    private float pixelDelay => Random.Range(0.05f, 0.15f);
    private float thoughtDelay => Random.Range(0.5f, 10f);
    private float idleDelay => Random.Range(5f, 10f);
    
    public void InitializeChancks()
    {
        chanks = SplitMap(map, maxPerChunk: 1000);
        //Debug.Log("Chanks count: " + chanks.Count);
    }
    
    public static List<List<Vector2Int>> SplitMap(List<Vector2Int> map, int maxPerChunk = 1000)
    {
        // 1. Определяем границы всей карты
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        foreach (var p in map)
        {
            if (p.x < minX) minX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.x > maxX) maxX = p.x;
            if (p.y > maxY) maxY = p.y;
        }

        var rootBounds = new RectInt(minX, minY, maxX - minX + 1, maxY - minY + 1);
        var result = new List<List<Vector2Int>>();

        SplitRecursive(map, rootBounds, maxPerChunk, result);
        return result;
    }
    private static void SplitRecursive(List<Vector2Int> points, RectInt bounds, int maxPerChunk, List<List<Vector2Int>> result)
    {
        if (points.Count <= maxPerChunk)
        {
            result.Add(points);
            return;
        }

        // We divide along the longer side
        RectInt a, b;
        List<Vector2Int> pa = new List<Vector2Int>();
        List<Vector2Int> pb = new List<Vector2Int>();

        if (bounds.width >= bounds.height)
        {
            int midX = bounds.xMin + bounds.width / 2;
            a = new RectInt(bounds.xMin, bounds.yMin, midX - bounds.xMin, bounds.height);
            b = new RectInt(midX, bounds.yMin, bounds.xMax - midX, bounds.height);

            foreach (var p in points)
                (p.x < midX ? pa : pb).Add(p);
        }
        else
        {
            int midY = bounds.yMin + bounds.height / 2;
            a = new RectInt(bounds.xMin, bounds.yMin, bounds.width, midY - bounds.yMin);
            b = new RectInt(bounds.xMin, midY, bounds.width, bounds.yMax - midY);

            foreach (var p in points)
                (p.y < midY ? pa : pb).Add(p);
        }

        SplitRecursive(pa, a, maxPerChunk, result);
        SplitRecursive(pb, b, maxPerChunk, result);
    }
    
    public void StartFindHumanaizedBot()
    {
        StopAllCoroutines();
        InitializeChancks();
        if (chanks.Count == 1 || Random.value < 0.2f)
            StartCoroutine(StartFindNearest());
        else
            StartCoroutine(StartFindHumanaized());
    }

    private IEnumerator StartFindHumanaized()
    {
        yield return new WaitForSeconds(idleDelay);
        
        chanks.Shuffle();

        foreach (var chank in chanks)
        {
            List<Vector2Int> localMap = BuildSnakeOrder(chank);

            foreach (var pixel in localMap)
            {
                targetPosition = GetPositionByCoords(pixel);
                yield return MoveToTargetSmooth();
                //yield return MoveToTargetEased();
                OnColorDraw?.Invoke(pixel);
                yield return new WaitForSeconds(pixelDelay);

                // Sometimes he “fell into thought”
                if (Random.value < idleChance)
                    yield return new WaitForSeconds(thoughtDelay);
            }
        }

        yield return new WaitForSeconds(idleDelay);
        OnColorJarEnd?.Invoke();
    }

    IEnumerator MoveToTargetEased()
    {
        Vector3 endPos = targetPosition + StartImagePosition;
        Vector2 start = transform.position;
        
        float dist = Vector2.Distance(start, endPos);
        float speed = Mathf.Lerp(minSpeed, maxSpeed, Mathf.Clamp01(dist / 50f));
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed / Mathf.Max(dist, 0.01f);
            float eased = t * t * (3f - 2f * t);
            transform.position = Vector2.Lerp(start, endPos, eased);
            yield return null;
        }
        transform.position = endPos;
    }
    
    List<Vector2Int> BuildSnakeOrder(List<Vector2Int> pixels)
    {
        var byRow = pixels
            .GroupBy(p => p.y)
            .OrderBy(g => g.Key)
            .ToList();

        var result = new List<Vector2Int>(pixels.Count);
        bool leftToRight = true;

        foreach (var row in byRow)
        {
            var ordered = leftToRight
                ? row.OrderBy(p => p.x)
                : row.OrderByDescending(p => p.x);
            result.AddRange(ordered);
            leftToRight = !leftToRight;
        }
        return result;
    }
    
    private IEnumerator StartFindNearest()
    {
        yield return new WaitForSeconds(idleDelay);
        
        while (true)
        {
            targetPosition = GetNearestPixel(transform.position);
            
            if (targetPosition == null)
            {
                //Need next jar;
                yield return new WaitForSeconds(idleDelay);
                OnColorJarEnd?.Invoke();
                break;
            }
            
            yield return MoveToTargetSmooth();

            if (targetPosition == null)
                throw new Exception();
            
            OnColorDraw?.Invoke(pointToRemove);
            map.Remove(pointToRemove);
            yield return new WaitForSeconds(pixelDelay);

            // Sometimes he “fell into thought”
            if (Random.value < idleChance)
                yield return new WaitForSeconds(thoughtDelay);
        }
    }
    private IEnumerator MoveToTargetSmooth()
    {
        Vector3 endPos = targetPosition + StartImagePosition;
        float currentSpeed = 0f;
    
        while (Vector3.Distance(transform.position, endPos) > 0.001f)
        {
            float distanceLeft = Vector3.Distance(transform.position, endPos);
            
            float targetSpeed = distanceLeft < slowDownRadius 
                ? maxSpeed * (distanceLeft / slowDownRadius) 
                : maxSpeed;
        
            float rate = targetSpeed > currentSpeed ? acceleration : deceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);
            currentSpeed = Mathf.Max(currentSpeed, minSpeed);
            
            transform.position = Vector3.MoveTowards(
                transform.position,
                endPos,
                currentSpeed * Time.deltaTime
            );
            yield return null;
        }
    
        transform.position = endPos;
    }
    private Vector3 GetNearestPixel(Vector3 transformPosition)
    {
        Vector2Int best = new Vector2Int(int.MaxValue, int.MaxValue);
        int bestDist = int.MaxValue;
        Vector2 localPos = transformPosition-StartImagePosition;
        localPos *= 100; // Texture scale
        localPos += new Vector2(.5f,.5f);
        Vector2Int cuttentPixel = new Vector2Int((int)localPos.x, (int)localPos.y);
        
        foreach (Vector2Int point in map)
        {
            int dist = Mathf.Abs(point.x - cuttentPixel.x) + Mathf.Abs(point.y - cuttentPixel.y);
            if (dist < bestDist) 
            { 
                bestDist = dist; 
                best = point; 
            }
        }

        pointToRemove = best;
        
        return GetPositionByCoords(best);
    }

    public Vector3 GetPositionByCoords(Vector2Int pixel)
    {
        Vector3 result = new Vector3(pixel.x, pixel.y, 0);
        result += new Vector3(.5f,.5f, 0);
        result /= 100;
        
        return result;
    }
}
