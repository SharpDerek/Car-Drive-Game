using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCameraFollow : MonoBehaviour {

    // replace with cinemachine behaviour at some point

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Transform target;

    void Update() {
        transform.position = target.position + offset;
    }

    void OnValidate() {
        transform.position = target.position + offset;
    }
}
