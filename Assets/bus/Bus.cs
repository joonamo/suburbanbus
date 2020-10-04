using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
    public float accelerator = 0.0f;
    public float steering = 0.0f;

    public float accelMult = 10.0f;
    public float steerMult = 100.0f;
    public float maxVelocity = 10.0f;
    public float tractionMult = 1.0f;
    public AnimationCurve traction;
    public AnimationCurve accelCurve;
    public AnimationCurve rotateCurve;
    public float brakeMultiplier = 5.0f;
    public AnimationCurve brakeCurve;
    public Rigidbody rb;
    private GameManager gm;

    public AudioSource crashSound;
    // Start is called before the first frame update
    void Start()
    {
       rb = GetComponent<Rigidbody>(); 
       gm = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate() 
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float speed = horizontalVelocity.magnitude;
        float velPercent = speed / maxVelocity;
        float velDir = Vector3.Dot(transform.forward, horizontalVelocity);
        float velCorrectness = Mathf.Abs(velDir);

        rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * speed + Vector3.up * rb.velocity.y, traction.Evaluate(velPercent));
        if (accelerator != 0) {
            if (speed < 0.5 || Mathf.Sign(velDir) == Mathf.Sign(accelerator)) {
                float accel = accelCurve.Evaluate(velPercent) * accelerator * accelMult;
                rb.AddForce(transform.forward * accel, ForceMode.VelocityChange);
            } else {
                rb.AddForce(transform.forward * -brakeMultiplier * brakeCurve.Evaluate(velPercent));
            }
        } else {
            rb.AddForce(transform.forward * -2.0f);
        }

        if (steering != 0) {
            float steerAmount = rotateCurve.Evaluate(velPercent) * steering * steerMult * Mathf.Sign(velDir);
            transform.Rotate(Vector3.up * steerAmount, Space.Self);
        }
    }

    void OnCollisionEnter(Collision other) {
        if (gm && gm.gameState != GameState.intro) {
            crashSound.Play();
        }
    }
}
