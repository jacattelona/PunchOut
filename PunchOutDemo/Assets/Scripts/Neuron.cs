using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron
{
    public const float VOLTAGE_PER_TICK_FULL_OPEN = 10;
    public const float THRESHOLD_POTENTIAL = -55;
    public const float ACTIVATION_POTENTIAL = 40;
    public const float RESTING_POTENTIAL = -70;
    public const float REFACTORY_PERIOD_MS = 2;
    public const float CHANNEL_DEGREDATION_AMOUNT = 0.1f;
    public const float VOLTAGE_DEGREDATION_AMOUNT = 0.01f;

    private List<Synapse> incoming, outgoing;

    private float internalVoltage;
    private float channelOpenPercent;

    private float activationStartTime;


    public Neuron()
    {
        internalVoltage = RESTING_POTENTIAL;
        channelOpenPercent = 0;
        activationStartTime = 0;
        incoming = new List<Synapse>();
        outgoing = new List<Synapse>();
    }


    public void StimulateChannels(float stimulantAmount)
    {
        if (IsReadyToFire())
        {
            channelOpenPercent = Mathf.Min(channelOpenPercent + stimulantAmount, 1.0f);
        }
    }

    public void InhibitChannels(float inhibitorAmount)
    {
        if (IsReadyToFire())
        {
            channelOpenPercent = Mathf.Max(channelOpenPercent - inhibitorAmount, 0.0f);
        }
    }

    public double GetInternalVoltage()
    {
        return internalVoltage;
    }

    /**
     * Draws in ions, increasing its internal voltage if it is ready to fire.
     * @return True if the neuron has exceeded the threshold potential.
     */
    public bool PullInIons()
    {
        if (IsReadyToFire())
        {
            internalVoltage += VOLTAGE_PER_TICK_FULL_OPEN * channelOpenPercent;
            InhibitChannels(CHANNEL_DEGREDATION_AMOUNT);
            internalVoltage = Mathf.Max(RESTING_POTENTIAL, internalVoltage - VOLTAGE_DEGREDATION_AMOUNT);
            if (internalVoltage >= THRESHOLD_POTENTIAL)
            {
                Fire();
                return true;
            }
            else
            {
                NoFire();
                return false;
            }

        }
        NoFire();
        return false;
    }

    public Synapse AddOutgoingConnection(Neuron to)
    {
        System.Random r = new System.Random();
        Synapse connection = new Synapse(this, (float) r.NextDouble());
        connection.SetTo(to);
        connection.SetConnectionType(Synapse.Type.NORMAL);
        outgoing.Add(connection);
        to.AddIncomingConnection(connection);
        return connection;
    }

    public void AddIncomingConnection(Synapse from)
    {
        incoming.Add(from);
    }

    public void NoFire()
    {
        foreach (Synapse synapse in incoming)
        {
            synapse.ToFired(false);
        }

        foreach (Synapse synapse in outgoing)
        {
            synapse.NoFire();
        }
    }


    public void Fire()
    {
        channelOpenPercent = 0;
        activationStartTime = Time.time * 1000;
        internalVoltage = RESTING_POTENTIAL;
        // TODO: broadcast activation
        foreach (Synapse synapse in incoming)
        {
            synapse.ToFired(true);
        }

        foreach (Synapse synapse in outgoing)
        {
            synapse.Fire();
        }
    }

    private bool IsReadyToFire()
    {
        return (Time.time * 1000) - activationStartTime >= REFACTORY_PERIOD_MS;
    }
}
