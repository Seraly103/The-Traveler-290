using UnityEngine;
using Yarn.Unity;
public class KeyTrigger : MonoBehaviour
{
    public GameObject keyPrefab;
    public Transform spawnPoint;
    public Transform groundPoint;

    [YarnCommand("dropKey")]

    public void DropKey()
    {
        GameObject key = Instantiate(keyPrefab, spawnPoint.position, Quaternion.identity);
        StartCoroutine(DropAnimation(key));
    }

    System.Collections.IEnumerator DropAnimation(GameObject key)
    {
        float time = 0;
        float duration = 0.5f;

        Vector3 start = key.transform.position;
        Vector3 end = groundPoint.position;

        while (time < duration)
        {
            key.transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        key.transform.position = end;
    }

}
