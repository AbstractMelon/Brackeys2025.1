using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Light))]
public class FlickerLight : MonoBehaviour {

    private Light light;
    private float baseIntensity;
    [Range(0f, 1f)]
    public float flickerChance = 0.05f; // Slider to control flicker chance
    public float updateDelay = 0.1f; // Slider to control update delay

    private float nextUpdate;

    void Start() {
        light = GetComponent<Light>();
        baseIntensity = light.intensity;
        nextUpdate = Time.time + updateDelay;
    }

    void Update() {
        if (Time.time > nextUpdate) {
            light.enabled = Random.value > (1f - flickerChance);
            nextUpdate = Time.time + updateDelay;
        }
    }
}

