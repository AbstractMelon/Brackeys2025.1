using UnityEngine;

public class RandomColor : MonoBehaviour
{
    [Header("Options")]
    public bool changeOnStart = true;
    public bool useMaterialsInstead = false;
    public Material[] materialOptions;

    void Start()
    {
        if(changeOnStart) ApplyRandom();
    }

    public void ApplyRandom()
    {
        if(useMaterialsInstead && materialOptions.Length > 0)
        {
            GetComponent<Renderer>().material = materialOptions[Random.Range(0, materialOptions.Length)];
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(
                Random.value,
                Random.value,
                Random.value
            );
        }
    }
}