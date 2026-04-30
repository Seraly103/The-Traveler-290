using UnityEngine;
using Yarn.Unity;
using System.Collections;
public class KeyTrigger : MonoBehaviour
{
    public GameObject keyPrefab;
    public Transform spawnPoint;
    public Transform groundPoint;
    public static KeyTrigger instance;

    void Awake()
    {
        instance = this;
    }

    [YarnCommand("dropKey")]
    public static void DropKeyCommand()
    {
        if (instance != null)
        {
            instance.DropKey();
        }
        else
        {
            Debug.LogError("No KeyDropper in scene!");
        }
    }

    public void DropKey()
    {
        GameObject key = Instantiate(keyPrefab, spawnPoint.position, Quaternion.identity);
        StartCoroutine(DropAnimation(key));
    }

    IEnumerator DropAnimation(GameObject key)
    {
        float time = 0f;
        float duration = 0.5f;

        Vector3 start = key.transform.position;
        Vector3 end = groundPoint.position;

        while (time < duration)
        {
            float t = time / duration;
            key.transform.position = Vector3.Lerp(start, end, t);
            time += Time.deltaTime;
            yield return null;
        }

        key.transform.position = end;
    }
    

}
