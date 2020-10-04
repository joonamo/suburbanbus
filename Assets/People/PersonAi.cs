using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAi : MonoBehaviour
{
  bool isWaiting = true;
  int targetStop = 0;
  GameManager gm;
  GameObject closestStop;
  Vector3 wanderTarget;
  Vector3 currentDirection;
  float currentSpeed = 0.0f;
  public float wanderDistance = 10.0f;
  public float maxWanderSpeed = 2.0f;
  public float minWanderSpeed = 1.0f;
  public float maxRotateSpeed = 0.2f;
  public float minRotateSpeed = 0.1f;
  CharacterController cc;

  public void OnInitialize(bool isWaiting, int targetStop, GameManager gm, GameObject closestStop)
  {
    this.isWaiting = isWaiting;
    this.targetStop = targetStop;
    this.gm = gm;
    this.closestStop = closestStop;

    var myMesh = GetComponent<MeshRenderer>();
    foreach (var material in myMesh.materials)
    {
      material.SetColor("_Color", gm.stopColors[targetStop]);
    }

    if (!isWaiting)
    {
      wanderTarget = closestStop.transform.position;
      currentSpeed = maxWanderSpeed * Random.Range(2.0f, 4.0f);
    }
    else
    {
      wanderTarget = transform.position;
    }
  }

  // Start is called before the first frame update
  void Start()
  {
    cc = GetComponent<CharacterController>();
    currentDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)).normalized;
    Random.Range(minWanderSpeed, maxWanderSpeed);
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    if (isWaiting)
    {
      // Wandering
      if (
        gm.gameState == GameState.game &&
        Vector3.Distance(transform.position, gm.playerBus.transform.position) < 10.0
        )
      {
        wanderTarget = gm.playerBus.transform.position;
        currentSpeed = maxWanderSpeed * 3.0f;
      }
      else if (Vector3.Distance(transform.position, closestStop.transform.position) > wanderDistance)
      {
        wanderTarget = closestStop.transform.position;
      }
      else if (Vector3.Distance(transform.position, wanderTarget) < 2.0f || cc.velocity.magnitude < 0.1f)
      {
        var position = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
        wanderTarget = closestStop.transform.position + position.normalized * Random.Range(wanderDistance * 0.3f, wanderDistance);
        currentSpeed = Random.Range(minWanderSpeed, maxWanderSpeed);
      }
    }
    else
    {
      // Going home
      if (Vector3.Distance(transform.position, wanderTarget) < 2.0)
      {
        closestStop.GetComponent<AudioSource>().Play();
        Destroy(gameObject);
      }
    }

    var dirToTarget = (wanderTarget - transform.position).normalized;
    currentDirection = Vector3.Lerp(currentDirection, dirToTarget, Random.Range(minRotateSpeed, maxRotateSpeed)).normalized;

    cc.SimpleMove(currentDirection * currentSpeed);
  }

  void OnTriggerEnter(Collider other)
  {
    if (isWaiting && other.transform.tag == "Player")
    {
      gm.addPersonToBus(targetStop);
      Destroy(gameObject);
    }
  }
}
