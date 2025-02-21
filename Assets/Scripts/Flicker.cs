using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Light))]
public class FlickerLight : MonoBehaviour {

    private Light light;
    private float baseIntensity;
    [Range(0f, 1f)]
    public float flickerChance = 0.05f; // Slider to control flicker chance
    public float updateDelay = 0.1f; // Slider to control update delay

    public bool syncWithOtherLights = false; // New option to sync with other lights
    public Light[] otherLights; // Array of other lights to sync with

    public float minOffTime = 0.3f; // Minimum time the light will stay off
    public float maxOffTime = 1f; // Maximum time the light will stay off

    private float nextUpdate;
    private float nextOnTime;

    void Start() {
        light = GetComponent<Light>();
        baseIntensity = light.intensity;
        nextUpdate = Time.time + updateDelay;
        nextOnTime = Time.time + minOffTime;
    }

    void Update() {
        if (Time.time > nextUpdate) {
            bool flicker = Random.value > (1f - flickerChance);
            if (syncWithOtherLights) {
                foreach (Light otherLight in otherLights) {
                    otherLight.enabled = flicker;
                }
            }
            light.enabled = flicker;
            if (!flicker) {
                nextOnTime = Time.time + Random.Range(minOffTime, maxOffTime);
            }
            nextUpdate = Time.time + updateDelay;
        }

        if (!light.enabled && Time.time > nextOnTime) {
            light.enabled = true;
        }
    }
}

