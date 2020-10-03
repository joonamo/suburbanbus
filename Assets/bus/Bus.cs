using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
    public float accelMult = 10.0f;
    public float steerMult = 100.0f;
    public float maxVelocity = 10.0f;
    public float tractionMult = 1.0f;
    public AnimationCurve traction;
    public AnimationCurve accelCurve;
    public AnimationCurve rotateCurve;
    public float brakeMultiplier = 5.0f;
    public AnimationCurve brakeCurve;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
       rb = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate() 
    {
        float accelerator = Input.GetAxis("Accelerate");
        float steering = Input.GetAxis("Steering");
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float speed = horizontalVelocity.magnitude;
        float velPercent = speed / maxVelocity;
        float velDir = Vector3.Dot(transform.forward, horizontalVelocity);
        float velCorrectness = Mathf.Abs(velDir);

        rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * speed + Vector3.up * rb.velocity.y, traction.Evaluate(velPercent));
        if (accelerator != 0) {
            Debug.Log(Mathf.Sign(velDir) + "" + Mathf.Sign(accelerator));
            if (speed < 0.5 || Mathf.Sign(velDir) == Mathf.Sign(accelerator)) {
                float accel = accelCurve.Evaluate(velPercent) * accelerator * accelMult;
                rb.AddRelativeForce(Vector3.forward * accel, ForceMode.VelocityChange);
                Debug.DrawLine(transform.position, transform.position + transform.up * accel * 10);
            } else {
                rb.AddRelativeForce(Vector3.forward * -brakeMultiplier * brakeCurve.Evaluate(velPercent));
            }
        } else {
            rb.AddRelativeForce(Vector3.forward * -2.0f);
        }

        Debug.DrawLine(transform.position, transform.position + transform.forward * velPercent * 20);

        if (steering != 0) {
            float steerAmount = rotateCurve.Evaluate(velPercent) * steering * steerMult;
            Debug.DrawLine(transform.position, transform.position + transform.right * steerAmount);
            transform.Rotate(Vector3.up * steerAmount, Space.Self);
        }
    }
}
