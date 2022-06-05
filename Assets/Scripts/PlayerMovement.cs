using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 mousePosition;
    public float moveSpeed = 0.1f;

    void Update () {
            mousePosition = Input.mousePosition;
            mousePosition = Camera.main!.ScreenToWorldPoint(mousePosition);
            transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed * Time.deltaTime);
                
            mousePosition.Normalize();
            var rotZ = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90);
    }
}
