using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 0.1f;
    private Animator _animator;
    public List<Field> touchingFields;

    private static PlayerMovement _instance;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Death = Animator.StringToHash("Death");

    public static PlayerMovement Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PlayerMovement>();
            return _instance;
        }
    }
    
    private void Awake() => _animator = GetComponentInChildren<Animator>();
    
    public void Die()
    {
        _animator.speed = 1f;
        _animator.SetTrigger(Death);
    }


    void Update ()
    {
        if (!GameManager.Instance?.Playing ?? false) return;
        
            var mousePosition = Input.mousePosition;
            mousePosition = Camera.main!.ScreenToWorldPoint(mousePosition);
            
            if(GameManager.Instance){ 
                mousePosition = new Vector2(Mathf.Clamp(mousePosition.x, WorldBounds.Instance.MINBounds.x,WorldBounds.Instance.MAXBounds.x),
                Mathf.Clamp(mousePosition.y, WorldBounds.Instance.MINBounds.y,WorldBounds.Instance.MAXBounds.y) );
            }

            var distance = Vector2.Distance(transform.position, mousePosition);
            
            _animator.SetFloat(Speed, distance);

            if (distance < moveSpeed/10f) distance = 1;

            var modifier = touchingFields.Count > 0 ? touchingFields.Average(field => field.stickyModifier) : 1f;


            _animator.speed = modifier;
            
            transform.position = Vector2.Lerp(transform.position, mousePosition,  modifier * moveSpeed * Time.deltaTime/distance);
                
            Vector2 direction = mousePosition - transform.position;

            var angle = Vector2.SignedAngle(Vector2.right, direction) + 90;
            
            
            
            transform.eulerAngles = new Vector3 (0, 0, Mathf.LerpAngle(transform.eulerAngles.z, angle, moveSpeed * Time.deltaTime/distance));
            
    }
}
