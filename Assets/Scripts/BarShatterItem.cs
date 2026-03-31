using UnityEngine;
using System.Collections;

public class BarShatterItem : MonoBehaviour
{
    [SerializeField] private GameObject destroyTarget;
    [SerializeField] private GameObject shatterEffect;
    [SerializeField] private AudioClip shatterSfx;
    [SerializeField] private float shatterVolume = 1f;
    [SerializeField] private Animator animator;
    [SerializeField] private string shatterTriggerName = "Shatter";
    [SerializeField] private float destroyDelay = 0f;

    private bool shattered;

    public void Shatter()
    {
        if (shattered)
        {
            return;
        }

        shattered = true;

        if (shatterEffect != null)
        {
            Instantiate(shatterEffect, transform.position, Quaternion.identity);
        }

        if (shatterSfx != null)
        {
            AudioSource.PlayClipAtPoint(shatterSfx, transform.position, shatterVolume);
        }

        if (animator != null && !string.IsNullOrWhiteSpace(shatterTriggerName))
        {
            animator.SetTrigger(shatterTriggerName);
        }

        GameObject target = destroyTarget != null ? destroyTarget : gameObject;
        if (destroyDelay > 0f)
        {
            StartCoroutine(DestroyAfterDelay(target));
            return;
        }

        Destroy(target);
    }

    private IEnumerator DestroyAfterDelay(GameObject target)
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(target);
    }
}