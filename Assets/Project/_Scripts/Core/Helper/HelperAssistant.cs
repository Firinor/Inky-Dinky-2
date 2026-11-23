using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperAssistant : MonoBehaviour
{
    public event Action<Vector3> OnColorDraw;
    public event Action OnColorJarEnd;
    
    public float correctColorDelay;
    public float speed;
    
    public SpriteRenderer Pen;
    public SpriteRenderer Avatar;
    
    public Color32 color32;
    public List<Vector2Int> map;
    public Vector2Int pointToRemove;
        
    private Vector3 targetPosition;
    private float timer;

    public static Vector3 StartImagePosition;
    
    public IEnumerator Start()
    {
        while (true)
        {
            targetPosition = GetNearestPixel(transform.position);
            
            if (targetPosition == null)
            {
                //Need next jar;
                yield return new WaitForSeconds(2f);
                OnColorJarEnd?.Invoke();
                break;
            }
            
            yield return MoveToTarget();

            if (targetPosition == null)
                throw new Exception();
            
            OnColorDraw?.Invoke(targetPosition);
            map.Remove(pointToRemove);
            yield return new WaitForSeconds(correctColorDelay);
        }
    }

    private IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition+StartImagePosition) > 0.1)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetPosition+StartImagePosition, 
                speed * Time.deltaTime
            );
            yield return null;
        }
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
        
        Vector3 result = new Vector3(best.x, best.y, 0);
        result -= new Vector3(.5f,.5f, 0);
        result /= 100;
        
        return result;
    }
}
