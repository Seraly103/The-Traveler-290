using UnityEngine;
using System.Collections;

public class PopUpAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float duration = 0.5f;
    private Coroutine currentAnimation;

    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        PlayOpen();
    }

    public void PlayOpen()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(Scale(Vector3.zero, originalScale));
    }


    public void PlayClose()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(CloseRoutine());
    }

    IEnumerator Scale(Vector3 start, Vector3 end)
    {
        float time = 0;
        transform.localScale = start;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);

            transform.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.localScale = end;
    }

    IEnumerator CloseRoutine(System.Action onComplete = null)
    {
        yield return StartCoroutine(Scale(originalScale, Vector3.zero));

        onComplete?.Invoke(); 

        gameObject.SetActive(false);
    }

}
