using UnityEngine;

public class BarEndShatterZone : MonoBehaviour
{
    [SerializeField] private string[] nameContainsFilters;
    [SerializeField] private bool destroyMatchedObjectIfNoBarShatterItem = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryShatter(other.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryShatter(other.transform);
    }

    private void TryShatter(Transform otherTransform)
    {
        BarShatterItem shatterItem = otherTransform.GetComponentInParent<BarShatterItem>();
        if (shatterItem != null)
        {
            shatterItem.Shatter();
            return;
        }

        if (!destroyMatchedObjectIfNoBarShatterItem)
        {
            return;
        }

        Transform matchedTransform = FindMatchingTransform(otherTransform);
        if (matchedTransform != null)
        {
            Destroy(matchedTransform.gameObject);
        }
    }

    private Transform FindMatchingTransform(Transform startTransform)
    {
        if (nameContainsFilters == null || nameContainsFilters.Length == 0)
        {
            return null;
        }

        Transform current = startTransform;
        while (current != null)
        {
            string currentName = current.name;
            for (int i = 0; i < nameContainsFilters.Length; i++)
            {
                string filter = nameContainsFilters[i];
                if (string.IsNullOrWhiteSpace(filter))
                {
                    continue;
                }

                if (currentName.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return current;
                }
            }

            current = current.parent;
        }

        return null;
    }
}