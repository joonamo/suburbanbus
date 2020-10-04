using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBus : MonoBehaviour
{
  public Bus targetBus;
  public WayPoint targetPoint;
  private WayPoint prevTarget;
  public AnimationCurve steerCurve;
  public float dangerQueryDist = 15.0f;
  private float recoveryMode = 0.0f;
  public float recoveryTime = 15.0f;

  // Start is called before the first frame update
  void Start()
  {
    
  }

  // Update is called once per frame
  void FixedUpdate()
  {

    if (recoveryMode > 0.0f)
    {
      recoveryMode = recoveryMode - Time.fixedDeltaTime;
      targetBus.accelerator = -1.0f;
    }
    else
    {
      Vector3 toTarget = targetPoint.transform.position - targetBus.transform.position;
      if (toTarget.magnitude < 5.0f)
      {
        var possibleTargets = new List<WayPoint>();
        foreach (var target in targetPoint.availablePoints)
        {
          if (target != prevTarget)
          {
            possibleTargets.Add(target);
          }
        }
        var tempPrevCurrentTarget = targetPoint;
        if (possibleTargets.Count == 0)
        {
          targetPoint = prevTarget;
        }
        else
        {
          targetPoint = possibleTargets[Random.Range(0, possibleTargets.Count)];
        }
        prevTarget = tempPrevCurrentTarget;
      }
      float dot = Vector3.Dot(targetBus.transform.right, toTarget.normalized);
      RaycastHit hit;
      float dangerRight = 0.0f;
      float dangerLeft = 0.0f;
      if (Physics.Raycast(
          targetBus.transform.position,
          Vector3.Lerp(targetBus.transform.forward, targetBus.transform.right, 0.4f),
          out hit,
          dangerQueryDist))
      {
        Debug.DrawLine(targetBus.transform.position, hit.point);
        dangerRight = 1.0f - hit.distance / dangerQueryDist;
        //   Debug.DrawLine(targetBus.transform.position, targetBus.transform.position + Vector3.Lerp(targetBus.transform.forward, targetBus.transform.right, 0.4f) * dangerQueryDist);
      }

      if (Physics.Raycast(
          targetBus.transform.position,
          Vector3.Lerp(targetBus.transform.forward, -targetBus.transform.right, 0.4f),
          out hit,
          dangerQueryDist))
      {
        //   Debug.DrawLine(targetBus.transform.position, targetBus.transform.position + Vector3.Lerp(targetBus.transform.forward, -targetBus.transform.right, 0.4f) * dangerQueryDist);
        Debug.DrawLine(targetBus.transform.position, hit.point);
        dangerLeft = 1.0f - hit.distance / dangerQueryDist;
      }
      targetBus.steering = steerCurve.Evaluate(Mathf.Abs(dot)) * Mathf.Sign(dot);
      if (dangerLeft > 0 && dangerLeft > dangerRight) {
        targetBus.steering = Mathf.Clamp(targetBus.steering + 1.5f, -1.0f, 1.0f);
      } else if (dangerRight > 0) {
        targetBus.steering = Mathf.Clamp(targetBus.steering - 1.5f, -1.0f, 1.0f);
      } 

      float pointingAt = 0.1f + Mathf.Abs(Vector3.Dot(targetBus.transform.forward, toTarget.normalized)) * 0.5f;
      targetBus.accelerator = pointingAt;

      if (targetBus.rb.velocity.magnitude < 1.0f)
      {
        recoveryMode -= Time.fixedDeltaTime;
        if (recoveryMode < -1.0f)
        {
          recoveryMode = recoveryTime;
          targetBus.steering = Mathf.Sign(Random.Range(-1, 1));
        }
      }
      else
      {
        recoveryMode = 0.0f;
      }
    }
    Debug.DrawLine(targetBus.transform.position, targetBus.transform.position + targetBus.transform.right * targetBus.steering * 10);
  }
}
