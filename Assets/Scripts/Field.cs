using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] private FieldState fieldState = FieldState.Neutral;
    [SerializeField] private float maxSize = 5f;
    [SerializeField] private float growTime = 1f;
    [SerializeField] private float dyingTime = 1f;
    [SerializeField] private float waitingTime = 1f;
    [SerializeField] public float stickyModifier = 1f;
    [SerializeField] public int scoreModifier = 1;
    
    private float _sizeModificator;
    private bool _hasPlayer;
    public List<GameObject> touchingFields;
    private SpriteRenderer _spriteRenderer;
    private Sprite _defaultSprite;
    private CircleCollider2D _collider;
    [SerializeField] private Sprite waitingSprite;
    private bool _waiting;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();
        _defaultSprite = _spriteRenderer.sprite;
    }

    private void Start()
    {
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        StartCoroutine(SpawnSignaling());
        
        var elapsedTime = 0f;
        var maxScale = new Vector3(maxSize, maxSize, maxSize);
        var startScale = transform.localScale;
        while(touchingFields.Count <= 0 && elapsedTime < growTime)
        {
            transform.localScale = Vector3.Lerp(startScale, maxScale, elapsedTime/growTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        dyingTime += growTime - elapsedTime;
        
        yield return new WaitUntil(() => !_waiting);
        
        StartCoroutine(Dying());
    }

    private IEnumerator SpawnSignaling()
    {
        _waiting = true;
        _spriteRenderer.sprite = waitingSprite;
        yield return new WaitForSecondsRealtime(waitingTime);
        _spriteRenderer.sprite = _defaultSprite;

        var mask = new ContactFilter2D
        {
            layerMask = LayerMask.NameToLayer("Player")
        };
        
        if (Physics2D.OverlapCollider(_collider, mask, new List<Collider2D>()) > 0)
        {
            PlayerEntered();
        }
        
        _waiting = false;
    }

    private IEnumerator Dying()
    {
        yield return new WaitForSecondsRealtime(dyingTime);
        GameManager.Instance?.CurrentFields.Remove(gameObject);
        if(_hasPlayer)  GameManager.Instance.ChangeState();
        touchingFields.ForEach(field =>
        {
            if(field && gameObject)
                field.GetComponent<Field>().touchingFields.Remove(gameObject);
        });
        PlayerMovement.Instance.touchingFields.Remove(this);
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_waiting)
        {
            PlayerEntered();
        }
        if(other.CompareTag("Field")) touchingFields.Add(other.gameObject);
        
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !_waiting)
        {
            PlayerMovement.Instance.touchingFields.Remove(this);
            _hasPlayer = false;
            GameManager.Instance?.ChangeState();
        }
    }

    private void PlayerEntered()
    {
        if(fieldState == FieldState.Death) GameManager.Instance.EndGame();
            
        PlayerMovement.Instance.touchingFields.Add(this);
            
        _hasPlayer = true;
        GameManager.Instance?.ChangeState(fieldState);
    }
}
