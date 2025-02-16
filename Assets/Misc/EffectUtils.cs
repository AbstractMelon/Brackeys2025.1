using UnityEngine;

public class EffectUtility : MonoBehaviour
{
    public static void ShakeCamera(float duration, float magnitude)
    {
        CameraShake shake = Camera.main.GetComponent<CameraShake>();
        if (shake == null)
        {
            shake = Camera.main.gameObject.AddComponent<CameraShake>();
        }
        shake.Shake(duration, magnitude);
    }

    public static void SpawnParticleEffect(GameObject particlePrefab, Vector3 position, float destroyDelay = 2f)
    {
        GameObject effect = Instantiate(particlePrefab, position, Quaternion.identity);
        Destroy(effect, destroyDelay);
    }
}

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalPos;
    private float _shakeDuration;

    public void Shake(float duration, float magnitude)
    {
        _originalPos = transform.localPosition;
        _shakeDuration = duration;
        StartCoroutine(DoShake(magnitude));
    }

    private System.Collections.IEnumerator DoShake(float magnitude)
    {
        while (_shakeDuration > 0)
        {
            transform.localPosition = _originalPos + Random.insideUnitSphere * magnitude;
            _shakeDuration -= Time.deltaTime;
            yield return null;
        }
        transform.localPosition = _originalPos;
    }
}
