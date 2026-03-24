using System;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [Serializable]
    private class ParallaxLayer
    {
        public Transform layer;
        public float horizontalMultiplier = 0.5f;
        public float verticalMultiplier = 0f;

        [HideInInspector] public Vector3 startPosition;
    }

    [Header("Movement Source")]
    [SerializeField] private Transform movementSource;

    [Header("Auto Setup")]
    [SerializeField] private bool autoRegisterChildren = true;
    [SerializeField] private bool includeInactiveChildren = false;
    [SerializeField] private bool onlyWhenLayerListEmpty = true;
    [SerializeField] private float autoHorizontalMultiplier = 0.5f;
    [SerializeField] private float autoVerticalMultiplier = 0f;

    [Header("Layers")]
    [SerializeField] private List<ParallaxLayer> layers = new List<ParallaxLayer>();

    private Vector3 sourceStartPosition;

    private void Awake()
    {
        if (!autoRegisterChildren)
        {
            return;
        }

        if (onlyWhenLayerListEmpty && layers.Count > 0)
        {
            return;
        }

        RegisterChildrenAsLayers();
    }

    private void Start()
    {
        if (movementSource == null)
        {
            Debug.LogWarning("ParallaxManager needs a Movement Source (World/Camera/Player transform).", this);
            enabled = false;
            return;
        }

        sourceStartPosition = movementSource.position;

        for (int i = 0; i < layers.Count; i++)
        {
            if (layers[i].layer == null)
            {
                continue;
            }

            if (layers[i].layer == movementSource)
            {
                Debug.LogWarning($"Parallax layer '{layers[i].layer.name}' is the same as Movement Source and will be ignored.", this);
                continue;
            }

            layers[i].startPosition = layers[i].layer.position;
        }
    }

    private void LateUpdate()
    {
        Vector3 sourceDelta = movementSource.position - sourceStartPosition;

        for (int i = 0; i < layers.Count; i++)
        {
            Transform layerTransform = layers[i].layer;
            if (layerTransform == null || layerTransform == movementSource)
            {
                continue;
            }

            layerTransform.position = new Vector3(
                layers[i].startPosition.x + (sourceDelta.x * layers[i].horizontalMultiplier),
                layers[i].startPosition.y + (sourceDelta.y * layers[i].verticalMultiplier),
                layers[i].startPosition.z
            );
        }
    }

    private void RegisterChildrenAsLayers()
    {
        Transform[] childTransforms = GetComponentsInChildren<Transform>(includeInactiveChildren);
        HashSet<Transform> existingLayers = new HashSet<Transform>();

        for (int i = 0; i < layers.Count; i++)
        {
            if (layers[i].layer != null)
            {
                existingLayers.Add(layers[i].layer);
            }
        }

        for (int i = 0; i < childTransforms.Length; i++)
        {
            Transform child = childTransforms[i];

            if (child == transform || child == movementSource || existingLayers.Contains(child))
            {
                continue;
            }

            layers.Add(new ParallaxLayer
            {
                layer = child,
                horizontalMultiplier = autoHorizontalMultiplier,
                verticalMultiplier = autoVerticalMultiplier
            });

            existingLayers.Add(child);
        }
    }
}