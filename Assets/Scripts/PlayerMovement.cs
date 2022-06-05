using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 mousePosition;
    public float moveSpeed = 0.1f;
    private Animator _animator;

    private static PlayerMovement _instance;
    public static PlayerMovement Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PlayerMovement>();
            return _instance;
        }
    }
    
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void Die()
    {
        _animator.SetTrigger("Death");
    }

    void Update ()
    {
        if (!GameManager.Instance.Playing) return;
        
            mousePosition = Input.mousePosition;
            mousePosition = Camera.main!.ScreenToWorldPoint(mousePosition);
            
            _animator.SetFloat("Speed", Vector3.Distance(transform.position,mousePosition));
            
            transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed * Time.deltaTime);
                
            // mousePosition.Normalize();
            // var rotZ = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90);

            Vector2 direction = mousePosition - transform.position;
            float angle = Vector2.SignedAngle(Vector2.right, direction);
            transform.eulerAngles = new Vector3 (0, 0, angle + 90);

            // transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, (angle + 90)%360f),
            //     moveSpeed * Time.deltaTime);
    }
}
