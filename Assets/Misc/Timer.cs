using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _timeLeft;
    private bool _isRunning;

    public void StartTimer(float duration)
    {
        _timeLeft = duration;
        _isRunning = true;
    }

    private void Update()
    {
        if (_isRunning)
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0)
            {
                _isRunning = false;
                Debug.Log("Timer finished!");
            }
        }
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    public float GetTimeLeft()
    {
        return _timeLeft;
    }
}
