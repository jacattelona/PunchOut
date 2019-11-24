using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownTimer
{
    private float cooldownTime;
    private float currentTimer;

    public CooldownTimer(float cooldownTime)
    {
        this.cooldownTime = cooldownTime;
        this.currentTimer = 0;
    }

    public void Start()
    {
        this.currentTimer = cooldownTime;
    }

    public bool IsOnCooldown()
    {
        return this.currentTimer > 0;
    }

    public void Update()
    {
        this.currentTimer -= Time.deltaTime;
    }

}
