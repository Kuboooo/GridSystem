using UnityEngine;

public class GameController : MonoBehaviour {
    private City city;
    private readonly float updateInterval = 10f; // Update every 10 seconds
    private float nextUpdateTime;

    private void Start() {
        city = new City(1000); // Starting population of 1000
        nextUpdateTime = Time.time + updateInterval;
    }

    private void Update() {
        if (!(Time.time >= nextUpdateTime)) return;
        city.UpdateCityStatus();
        nextUpdateTime += updateInterval;
    }
}