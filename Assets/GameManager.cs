using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<int> peopleWithTargets = new List<int>();
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
        var allBusStops = GameObject.FindObjectsOfType<BusStop>();
        foreach (var stop in allBusStops) {
            peopleWithTargets.Add(0);
        }
        stopsAmount = allBusStops.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
