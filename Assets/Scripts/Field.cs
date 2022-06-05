using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] private FieldState fieldState = FieldState.Neutral;
    [SerializeField] private float maxSize = 5f;
    
    private float _sizeModificator;
    private bool _hasPlayer;
    public List<GameObject> touchingFields;

    private void Update()
    {
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        while(!(_sizeModificator > maxSize))
        {
            transform.localScale *= _sizeModificator * Time.deltaTime;
            _sizeModificator += 1f;
            yield return 0;
        }
        
        if(_hasPlayer)  GameManager.Instance.ChangeState();
        touchingFields.ForEach(field => field.GetComponent<Field>().touchingFields.Remove(gameObject));
        GameManager.Instance.CurrentFields.Remove(gameObject);
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Player"))
        {
            _hasPlayer = true;
            GameManager.Instance.ChangeState(fieldState);
        }
        if(other.CompareTag("Field")) touchingFields.Add(other.gameObject);
        
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            _hasPlayer = false;
            GameManager.Instance.ChangeState();
        }
    }
}
