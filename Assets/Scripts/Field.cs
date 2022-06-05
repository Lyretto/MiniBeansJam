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
    
    private float _sizeModificator;
    private bool _hasPlayer;
    public List<GameObject> touchingFields;
    private SpriteRenderer _spriteRenderer;
    private Sprite _defaultSprite;
    private CircleCollider2D _collider;
    [SerializeField] private Sprite waitingSprite;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        StartCoroutine(Dying());
    }

    private IEnumerator SpawnSignaling()
    {
        _collider.enabled = false;
        _spriteRenderer.sprite = waitingSprite;
        yield return new WaitForSecondsRealtime(dyingTime);
        _spriteRenderer.sprite = _defaultSprite;
        _collider.enabled = true;
    }

    private IEnumerator Dying()
    {
        yield return new WaitForSecondsRealtime(dyingTime);
        GameManager.Instance.CurrentFields.Remove(gameObject);
        if(_hasPlayer)  GameManager.Instance.ChangeState();
        touchingFields.ForEach(field => field.GetComponent<Field>().touchingFields.Remove(gameObject));
        PlayerMovement.Instance.touchingFields.Remove(this);
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Player"))
        {
            if(fieldState == FieldState.Death) StartCoroutine(GameManager.Instance.Endgame());
            
            PlayerMovement.Instance.touchingFields.Add(this);
            
            _hasPlayer = true;
            GameManager.Instance.ChangeState(fieldState);
        }
        if(other.CompareTag("Field")) touchingFields.Add(other.gameObject);
        
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerMovement.Instance.touchingFields.Remove(this);
            _hasPlayer = false;
            GameManager.Instance.ChangeState();
        }
    }
}
