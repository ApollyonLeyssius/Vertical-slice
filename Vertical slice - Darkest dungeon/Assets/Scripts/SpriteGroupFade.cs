using UnityEngine;
using System.Collections;

public class SpriteGroupFade : MonoBehaviour
{
    SpriteRenderer[] sprites;

    void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    public void SetAlpha(float alpha)
    {
        foreach (var sr in sprites)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    public IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            float a = Mathf.Lerp(from, to, t / duration);
            SetAlpha(a);
            t += Time.deltaTime;
            yield return null;
        }
        SetAlpha(to);
    }
}
