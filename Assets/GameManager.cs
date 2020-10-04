using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
  intro,
  game,
  over
}

enum TutorialState
{
  collect,
  deliver,
  done
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

  public List<AudioClip> hellos;
  public List<AudioClip> thanks;
  public AudioClip gameover;
  public AudioClip tutorial1;
  public AudioClip tutorial2;
  public AudioSource audioOut;
  public AudioSource collectOut;
  public AudioSource musicOut;

  public TMPro.TextMeshProUGUI timeText;
  public TMPro.TextMeshProUGUI statusText;
  public TMPro.TextMeshProUGUI scoreText;
  public TMPro.TextMeshProUGUI multiplierText;

  TutorialState tutorialState = TutorialState.collect;
  public GameState gameState = GameState.intro;

  public Bus playerBus;
  public Camera introCamera;
  public Camera gameCamera;
  private AudioListener listener;

  public float personSpawnInterval = 1.0f;
  private float spawnCountdown = 0.0f;
  public float timer = 60.0f;

  public void addPersonToBus(int targetStop)
  {
    if (tutorialState == TutorialState.collect) {
      tutorialState = TutorialState.deliver;
      statusText.text = "Take the passangers to their stops!";
      audioOut.PlayOneShot(tutorial2);
    } else if (!audioOut.isPlaying) {
      audioOut.PlayOneShot(hellos[Random.Range(0, hellos.Count)]);
    }
    collectOut.Play();
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
    multiplierText.text = "x " + Mathf.Max(1, peopleInBus); 
  }

  public int stopReached(int stop)
  {
    int n = peopleWithTargets[stop];
    if (n > 0) {
      if (tutorialState == TutorialState.deliver)
      {
        tutorialState = TutorialState.done;
        statusText.text = "";
      }
      peopleTransported += peopleInBus * n;
      scoreText.text = "" + peopleTransported;
      peopleWithTargets[stop] = 0;
      updatePeopleInBus();
      timer += n * 0.9f;

      audioOut.PlayOneShot(thanks[Random.Range(0, thanks.Count)]);
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

    listener = gameCamera.GetComponent<AudioListener>();
    listener.enabled = false;
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

    musicOut.Play();
    listener.enabled = true;

    updatePeopleInBus();
    scoreText.text = "0";
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
            statusText.text = "Collect passangers!";
            audioOut.PlayOneShot(tutorial1);
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
            statusText.text = "Game Over!\nPress enter to play again";
            audioOut.PlayOneShot(gameover);
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
