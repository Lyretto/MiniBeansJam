using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int goodFieldScoreMultiplier = 5;
    [SerializeField] private int badFieldScoreMultiplier = -2;
    [SerializeField] private float defaultScoreTime = 1f;
    [SerializeField] private float badFieldScoreTime = .5f;
    [SerializeField] private float goodFieldScoreTime = .5f;
    [SerializeField] private int maxFields = 2;

    [SerializeField] private GameObject badFieldObject;
    [SerializeField] private GameObject goodFieldObject;
    [SerializeField] private TMP_Text scoreText;
    private int _score;
    private FieldState _fieldState = FieldState.Neutral;
    private bool _playing = true;
    private Vector2 _maxBounds;
    private Vector2 _minBounds;
    public bool spawning = true;
    [NonSerialized] public readonly List<GameObject> CurrentFields = new ();

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }
    
    void Start()
    {
        StartCoroutine(GatherScore());
        StartCoroutine(SpawnFields());
    }
    

    public void ChangeState(FieldState newState = FieldState.Neutral)
    {
        _fieldState = newState;
    }

    private IEnumerator SpawnFields()
    {
        var vertExtent = Camera.main!.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;
        
        _maxBounds = new Vector2(horzExtent / 2f, vertExtent / 2 );
        _minBounds = new Vector2(-horzExtent / 2f, -vertExtent / 2);

        yield return new WaitForSecondsRealtime(1f);
        
        while (spawning)
        {
            if (maxFields <= CurrentFields.Count) yield return new WaitForSecondsRealtime(2f);
            CurrentFields.Add(Instantiate(badFieldObject, RandomPosition(), Quaternion.identity));
            yield return new WaitForSecondsRealtime(1f);
        }
    }
    
    private IEnumerator GatherScore()
    {
        while (_playing)
        {

            var waitTime = defaultScoreTime;
            
            switch (_fieldState)
            {
                case FieldState.Bad:
                    waitTime -= badFieldScoreTime;
                    break;
                case FieldState.Good:
                    waitTime -= goodFieldScoreTime;
                    break;
            }

            yield return new WaitForSecondsRealtime(waitTime);

            switch (_fieldState)
            {
                case FieldState.Bad:
                    _score += badFieldScoreMultiplier;
                    break;
                case FieldState.Good:
                    _score += goodFieldScoreMultiplier;
                    break;
                case FieldState.Neutral:
                    _score++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            scoreText.text = "Score: " + _score;
        }
    }
    
    private Vector2 RandomPosition()
    {
        return new Vector2
        {
            x = Random.Range(_minBounds.x, _maxBounds.x),
            y = Random.Range(_minBounds.y, _maxBounds.y)
        };
    }
}

public enum FieldState
{
    Good,
    Neutral,
    Bad
}
