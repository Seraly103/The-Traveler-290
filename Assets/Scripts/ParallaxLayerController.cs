using UnityEngine;

public class ParallaxLayerController : MonoBehaviour
{
    [Header("Movement Source")]
    [SerializeField] private Transform movementSource;

    [Header("Parallax")]
    [SerializeField] private float horizontalMultiplier = 0.5f;
    [SerializeField] private float verticalMultiplier = 0f;

    private Vector3 initialLayerPosition;
    private Vector3 initialSourcePosition;

    private void Start()
    {
        initialLayerPosition = transform.position;

        if (movementSource == null)
        {
            Debug.LogWarning($"ParallaxLayerController on {name} is missing a movement source.", this);
            enabled = false;
            return;
        }

        if (movementSource == transform)
        {
            Debug.LogWarning($"ParallaxLayerController on {name} cannot use itself as Movement Source. Assign a shared source like World/Camera/Player.", this);
            enabled = false;
            return;
        }

        initialSourcePosition = movementSource.position;
    }

    private void LateUpdate()
    {
        Vector3 sourceDelta = movementSource.position - initialSourcePosition;

        transform.position = new Vector3(
            initialLayerPosition.x + (sourceDelta.x * horizontalMultiplier),
            initialLayerPosition.y + (sourceDelta.y * verticalMultiplier),
            initialLayerPosition.z
        );
    }
}