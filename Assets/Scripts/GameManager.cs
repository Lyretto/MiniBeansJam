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
    
    

    [SerializeField] private List<GameObject> fieldObjects;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text endScoreText;
    [SerializeField] private GameObject endUI;

    
    private int _score;
    private FieldState _fieldState = FieldState.Neutral;
    [NonSerialized] public bool Playing = true;
    private Vector2 _maxBounds;
    private Vector2 _minBounds;
    public bool spawning = true;
    [NonSerialized] public List<GameObject> CurrentFields = new ();

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
        StartGame();
    }

    public void StartGame()
    {
        endUI.SetActive(false);
        Playing = true;
        CurrentFields = new List<GameObject>();
        _score = 0;
        scoreText.text = "Score: 0";
        Time.timeScale = 1;
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
            
            CurrentFields.Add(Instantiate(fieldObjects[Random.Range(1,3)], RandomPosition(), Quaternion.identity));
            
            
            yield return new WaitForSecondsRealtime(1f);
        }
    }
    
    private IEnumerator GatherScore()
    {
        while (Playing)
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
                case FieldState.Death:
                    yield return 0;
                    break;
                    
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

    public IEnumerator Endgame()
    {
        PlayerMovement.Instance.Die();
        Playing = false;
        Time.timeScale = 0;
        
        yield return new WaitForSecondsRealtime(1f);
        
        endUI.SetActive(true);
        endScoreText.text = _score.ToString();
    }   
}

public enum FieldState
{
    Good,
    Neutral,
    Bad,
    Death
}
