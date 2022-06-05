using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Field : MonoBehaviour
{
    [SerializeField] private FieldState fieldState = FieldState.Neutral;
    [SerializeField] private float maxSize = 5f;
    [SerializeField] private float growTime = 1f;
    
    private float _sizeModificator;
    private bool _hasPlayer;
    public List<GameObject> touchingFields;

    private void Update()
    {
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        var counter = 0f;
        var maxScale = new Vector3(maxSize, maxSize, maxSize);
        while(touchingFields.Count <= 0 && counter < 1)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, maxScale, counter);
            counter += Time.deltaTime/growTime;
            
            Debug.Log(counter);
            yield return new WaitForSeconds(0.2f);
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
