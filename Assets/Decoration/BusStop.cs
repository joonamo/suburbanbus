using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusStop : MonoBehaviour
{
  public int idx;
  private GameManager gm;
  public GameObject whatIsPerson;
  public float personSpawnDistance = 2.0f;

  public void OnGameInit(int idx, GameManager gm)
  {
    this.idx = idx;
    this.gm = gm;
    var myMesh = transform.parent.gameObject.GetComponent<MeshRenderer>();
    foreach (var material in myMesh.materials)
    {
      material.SetColor("_Color", gm.stopColors[idx]);
    }

    spawnWaitingPerson();
    spawnWaitingPerson();
    spawnWaitingPerson();
  }

  public void spawnWaitingPerson()
  {
    var position = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
    position = transform.position + Vector3.up * 0.5f + position.normalized * personSpawnDistance;
    var spawnedPerson = Instantiate(whatIsPerson, position, Quaternion.identity);
    var personScript = spawnedPerson.GetComponent<PersonAi>();
    var targetStop = Random.Range(0, gm.stopColors.Count);
    while (targetStop == idx)
    {
      targetStop = Random.Range(0, gm.stopColors.Count);
    }
    personScript.OnInitialize(true, targetStop, gm, this.gameObject);
  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      Debug.Log("It's player!");
    }
  }
}
