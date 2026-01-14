using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BuffAndDebuffmanager : MonoBehaviour
{
    [SerializeField] private float moveDistance = 100f;
    [SerializeField] private float duration = 1f;

    private CanvasGroup canvasGroup;
    private Vector3 startPos;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        startPos = transform.localPosition;
    }

    public void Popup(int direction)
    {
        gameObject.SetActive(true);
        StartCoroutine(PopupRoutine(direction));
    }

    private IEnumerator PopupRoutine(int direction)
    {
        float time = 0f;

        Vector3 targetOffset = Vector3.zero;

        // 1 = left up, 2 = up, 3 = right up
        switch (direction)
        {
            case 1:
                targetOffset = new Vector3(-moveDistance, moveDistance, 0);
                break;
            case 2:
                targetOffset = new Vector3(0, moveDistance, 0);
                break;
            case 3:
                targetOffset = new Vector3(moveDistance, moveDistance, 0);
                break;
        }

        Vector3 start = startPos;
        Vector3 end = startPos + targetOffset;

        // Fade in
        canvasGroup.alpha = 1f;

        while (time < duration)
        {
            float t = time / duration;

            
            transform.localPosition = Vector3.Slerp(start, end, Mathf.SmoothStep(0, 1, t));

            // Fade out
            canvasGroup.alpha = 1f - t;

            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
