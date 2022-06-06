using System.Collections;
using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text pointText;
    [SerializeField] private float disappearTime;
    [SerializeField] private Color posColor;
    [SerializeField] private Color negColor;
    private int _score;
    
    public static GameObject Create(GameObject popupPrefab, Vector3 position, int points)
    {
        var popup = Instantiate(popupPrefab, position, Quaternion.identity);
        popup.GetComponent<ScorePopup>().Setup(points);
        return popup;
    }

    private void Setup(int points)
    {
        pointText.text = points.ToString("+#;-#;0");
        pointText.color = points > 0 ? posColor : negColor;
        _score = points;
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        var startPos = transform.position;
        var t = 0f;
        var goalDistance = Mathf.Sign(_score) * 2f;
        while (t < disappearTime)
        {
            t += Time.deltaTime;

            pointText.alpha = 1f - t / disappearTime;
            transform.position = Vector3.Slerp(startPos, new Vector2(startPos.x,startPos.y + goalDistance ),t);
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
        yield return 0;
    }
}
