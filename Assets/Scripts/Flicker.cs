using UnityEngine;  
[RequireComponent(typeof(Light))]  
public class FlickerLight : MonoBehaviour {  
    public float minIntensity = 0.2f;  
    public float maxIntensity = 1.5f;  
    public float flickerSpeed = 0.1f;  
    
    private Light _light;  
    private float _targetIntensity;  
    
    void Start() {  
        _light = GetComponent<Light>();  
    }  
    
    void Update() {  
        if (Mathf.Abs(_light.intensity - _targetIntensity) <= 0.05f) {  
            _targetIntensity = Random.Range(minIntensity, maxIntensity);  
        }  
        _light.intensity = Mathf.Lerp(_light.intensity, _targetIntensity, flickerSpeed * Time.deltaTime);  
    }  
}  