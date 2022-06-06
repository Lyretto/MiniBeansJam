using System;
using UnityEngine;

public class WorldBounds : MonoBehaviour
{
    
    [NonSerialized] public Vector2 MAXBounds;
    [NonSerialized] public Vector2 MINBounds;
    private static WorldBounds _instance;
    
    public static WorldBounds Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<WorldBounds>();
            return _instance;
        }
    }
    void Start()
    {
        var vertExtent = Camera.main!.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;
        
        MAXBounds = new Vector2(horzExtent, vertExtent );
        MINBounds = new Vector2(-horzExtent, -vertExtent);
    }
}