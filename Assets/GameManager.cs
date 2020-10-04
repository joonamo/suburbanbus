using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<int> peopleWithTargets = new List<int>();
    public List<Color> stopColors = new List<Color>();
    public int peopleInBus = 0;
    public int busCapacity = 30;
    public int peopleTransported = 0;
    public int stopsAmount = 0;

    public void addPersonToBus(int targetStop) {
        peopleWithTargets[targetStop] += 1;
        updatePeopleInBus();
    }

    private void updatePeopleInBus() {
        peopleInBus = 0;
        foreach (int targetAmount in peopleWithTargets) {
            peopleInBus += targetAmount;
        }
    }
    
    public void stopReached (int stop) {
        peopleTransported += peopleWithTargets[stop];
        peopleWithTargets[stop] = 0;
        updatePeopleInBus();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeGame();
    }

    void InitializeGame() {
        int numStopTypes = stopColors.Count;
        peopleTransported = 0;
        peopleWithTargets = new List<int>(numStopTypes); 

        var allBusStops = GameObject.FindObjectsOfType<BusStop>();
        int stopCount = allBusStops.GetLength(0);
        for (int i = 0; i < stopCount; ++i) {
            var temp = allBusStops[i];
            int swapIdx = Random.Range(0, stopCount);
            allBusStops[i] = allBusStops[swapIdx];
            allBusStops[swapIdx] = temp;
        }
        for (int i = 0; i < stopCount; ++i) {
            allBusStops[i].OnGameInit(i % numStopTypes, this);
        }
        stopsAmount = allBusStops.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
