using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public List<WayPoint> availablePoints = new List<WayPoint>();
    public GameObject carToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        var allPoints = GameObject.FindObjectsOfType<WayPoint>();
        foreach (var point in allPoints) {
            if (point == this) {
                continue;
            }
            Vector3 toOther = point.transform.position - transform.position;
            if (!Physics.Raycast(transform.position, toOther, toOther.magnitude, LayerMask.GetMask("Wall"))) {
                availablePoints.Add(point);
            }
        }

        var car = Instantiate(carToSpawn, transform.position, Quaternion.identity);
        car.GetComponent<AiBus>().targetPoint = this;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var point in availablePoints) {
            Debug.DrawLine(transform.position, point.transform.position);
        }
    }

  void OnDrawGizmos()
  {
    // Draw a yellow sphere at the transform's position
    Gizmos.color = Color.white;
    Gizmos.DrawSphere(transform.position, 0.3f);
  }
}
