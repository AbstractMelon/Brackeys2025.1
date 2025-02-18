using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : Item
{
    [SerializeField] private Transform needle;
    [SerializeField] private Transform player;

    private Vector3 startingNeedleLocalRotation;

    void Start()
    {
        startingNeedleLocalRotation = needle.localEulerAngles;
    }

    public override void Use()
    {
        base.Use();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        needle.localEulerAngles = startingNeedleLocalRotation + lookRotation.eulerAngles;
    }
}
