using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameState
{
  intro,
  game,
  over
}

public class GameManager : MonoBehaviour
{
  List<int> peopleWithTargets = new List<int>();
  public List<Color> stopColors = new List<Color>();
  public int peopleInBus = 0;
  public int busCapacity = 30;
  public int peopleTransported = 0;
  public int stopsAmount = 0;

  private List<BusStop> allBusStops = new List<BusStop>();

  public Canvas gameCanvas;
  public Canvas introCanvas;
  public List<TMPro.TextMeshProUGUI> targetTexts;

  public TMPro.TextMeshProUGUI timeText;
  public TMPro.TextMeshProUGUI statusText;
  public TMPro.TextMeshProUGUI scoreText;

  GameState gameState = GameState.intro;

  public Bus playerBus;
  public Camera introCamera;
  public Camera gameCamera;

  public float personSpawnInterval = 1.0f;
  private float spawnCountdown = 0.0f;
  public float timer = 60.0f;

  public void addPersonToBus(int targetStop)
  {
    peopleWithTargets[targetStop] += 1;
    updatePeopleInBus();
  }

  private void updatePeopleInBus()
  {
    peopleInBus = 0;
    int i = 0;
    foreach (int targetAmount in peopleWithTargets)
    {
      targetTexts[i].text = "" + targetAmount;
      peopleInBus += targetAmount;
      i++;
    }
  }

  public int stopReached(int stop)
  {
    int n = peopleWithTargets[stop];
    if (n > 0) {
      peopleTransported += peopleInBus * n;
      scoreText.text = "" + peopleTransported;
      peopleWithTargets[stop] = 0;
      updatePeopleInBus();
      timer += n;
    }
    return n;
  }

  // Start is called before the first frame update
  void Start()
  {
    introCamera.enabled = true;
    gameCamera.enabled = false;
    introCanvas.enabled = true;
    gameCanvas.enabled = false;
  }

  void InitializeGame()
  {
    int numStopTypes = stopColors.Count;
    peopleTransported = 0;
    peopleWithTargets = new List<int>();
    for (int i = 0; i < numStopTypes; ++i)
    {
      peopleWithTargets.Add(0);
    }

    allBusStops = new List<BusStop>(GameObject.FindObjectsOfType<BusStop>());
    stopsAmount = allBusStops.Count;
    for (int i = 0; i < stopsAmount; ++i)
    {
      var temp = allBusStops[i];
      int swapIdx = Random.Range(0, stopsAmount);
      allBusStops[i] = allBusStops[swapIdx];
      allBusStops[swapIdx] = temp;
    }
    for (int i = 0; i < stopsAmount; ++i)
    {
      allBusStops[i].OnGameInit(i % numStopTypes, this);
    }

    playerBus.enabled = true;
    playerBus.rb.constraints =
      RigidbodyConstraints.FreezePositionY 
      | RigidbodyConstraints.FreezeRotationX
      | RigidbodyConstraints.FreezeRotationZ;

    introCamera.enabled = false;
    gameCamera.enabled = true;
    introCanvas.enabled = false;
    gameCanvas.enabled = true;
  }

  // Update is called once per frame
  void Update()
  {
    switch (gameState)
    {
      case GameState.intro:
        {
          if (Input.GetButtonDown("Submit")) {
            InitializeGame();
            gameState = GameState.game;
          }
          break;
        }
      case GameState.game:
        {
          timer -= Time.deltaTime;
          if (timer <= 0.0f) {
            timer = 0.0f;
            playerBus.enabled = false;
            gameState = GameState.over;
            statusText.text = "Game Over!";
          }
          timeText.text = timer.ToString("#0.00");

          spawnCountdown -= Time.deltaTime;
          if (spawnCountdown < 0.0f) {
            allBusStops[Random.Range(0, stopsAmount)].spawnWaitingPerson();
            spawnCountdown = personSpawnInterval;
          }
          break;
        }
      case GameState.over:
        {
          if (Input.GetButtonDown("Submit"))
          {
            Application.LoadLevel(Application.loadedLevel);
          }
          break;
        }
    }
  }
}
