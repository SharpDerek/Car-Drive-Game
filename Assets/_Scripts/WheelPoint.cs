using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPoint : MonoBehaviour {

    [Header("Suspension")]
    [SerializeField]
    private float restLength;
    [SerializeField]
    private float springTravel;
    [SerializeField]
    private float springStiffness;
    [SerializeField]
    private float damperStiffness;

    [HideInInspector]
    public float steerAngle;

    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    private float springVelocity;
    private float springForce;
    private float damperForce;
    private float wheelAngle;

    private Vector2 force;

    private Vector3 suspensionForce;
    private Vector3 wheelVelocityLS;

    [Header("Wheel")]
    [SerializeField]
    private float wheelRadius = 0.5f;
    [SerializeField]
    private float steerTime = 10;

    private Rigidbody rb;
    private Wheel wheel;

    void Start() {
        rb = transform.root.GetComponent<Rigidbody>();

        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;

        springLength = springTravel + wheelRadius;
    }

    void Update() {
        wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);
    }

    void FixedUpdate() {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius)) {
            if (!hit.transform.CompareTag("Player")) {
                //Debug.Log(hit.transform.name + ", " + transform.name);
                lastLength = springLength;
                springLength = hit.distance - wheelRadius;
                springLength = Mathf.Clamp(springLength, minLength, maxLength);
                springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
                springForce = springStiffness * (restLength - springLength);
                damperForce = damperStiffness * springVelocity;

                wheelVelocityLS = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));

                force = new Vector2(
                    springForce,
                    wheelVelocityLS.x * 0.5f * springForce
                );

                suspensionForce = (springForce + damperForce) * transform.up;

                rb.AddForceAtPosition(suspensionForce + (force.x * 1 * transform.forward) + (force.y * -transform.right), hit.point);
            }
        }
        wheel.transform.localPosition = Vector3.down * springLength;
    }

    void OnValidate() {
        if (wheel == null) {
            wheel = GetComponentInChildren<Wheel>();
        }
        wheel.transform.localPosition = Vector3.down * (springTravel + wheelRadius);
    }

}
