using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFanTarget : FanTarget
{
    private Player player;
    private Vector3 fanVelocity;

    [SerializeField] private float damping = 0.90f;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public override void AddVelocity(Vector3 v)
    {
        fanVelocity += v;
    }

    private void Update()
    {
        fanVelocity *= damping;
        player.FanForce = fanVelocity;
    }

    public override void ResetFan()
    {
        fanVelocity = Vector3.zero;
        player.FanForce = Vector3.zero;
    }
}