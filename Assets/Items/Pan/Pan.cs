using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : ThrowableItem
{
    public float swingSpeed = 10f;
    public float swingDistance = 0.5f;

    private Vector3 initialPosition;

    protected override void Start()
    {
        base.Start();
        initialPosition = transform.position;
    }

    protected override void Use()
    {
        base.Use();
        StartCoroutine(Swing());
    }

    private IEnumerator Swing()
    {
        while (true)
        {
            float t = 0;
            while (t < swingSpeed)
            {
                t += Time.deltaTime;
                float x = Mathf.Sin(t * swingSpeed) * swingDistance;
                transform.position = initialPosition + new Vector3(x, 0, 0);
                yield return null;
            }
        }
    }
}
