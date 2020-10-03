using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public GameObject followTarget;
    public Rigidbody followBody;
    public float followSpeed = 0.5f;
    public float predictAmount = 2.0f;
    public AnimationCurve followDistance;
    public float minDistance = 10.0f;
    public float maxDistance = 20.0f;
    public float distanceSpeed = 10.0f;
    
    private Vector3 followPoint = new Vector3();
    private float currentFollowDistance = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        followPoint = followTarget.transform.position;
        followBody = followTarget.GetComponent<Rigidbody>();
        currentFollowDistance = minDistance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float targetSpeed = followBody.velocity.magnitude;
        followPoint = Vector3.Lerp(
            followPoint,
            followBody.position + followBody.velocity * predictAmount,
            followSpeed * Time.fixedDeltaTime);
        float distTarget = Mathf.Lerp(minDistance, maxDistance, followDistance.Evaluate(targetSpeed));
        currentFollowDistance = Mathf.Lerp(currentFollowDistance, distTarget, distanceSpeed * Time.fixedDeltaTime);
        transform.position = followPoint - transform.forward * currentFollowDistance;
    }
}
