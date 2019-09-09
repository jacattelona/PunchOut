using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synapse
{
    public enum Type
    {
        STIMULANT,
        INHIBITOR,
        NORMAL
    }

    private Neuron from, to;
    public float weight;
    private Type type;
    public bool fired;

    public Synapse(Neuron from, float weight)
    {
        this.from = from;
        this.weight = weight;
        this.type = Type.NORMAL;
        this.to = null;
    }


    public Neuron GetFrom()
    {
        return from;
    }

    public void SetFrom(Neuron from)
    {
        this.from = from;
    }

    public Neuron GetTo()
    {
        return to;
    }

    public void SetTo(Neuron to)
    {
        this.to = to;
    }

    public float GetWeight()
    {
        return weight;
    }

    public void SetWeight(float weight)
    {
        this.weight = weight;
    }

    public Type GetConnectionType()
    {
        return type;
    }

    public void SetConnectionType(Type type)
    {
        this.type = type;
    }

    public void NoFire()
    {
        fired = false;
    }

    public void Fire()
    {
        if (to == null)
        {
            return;
        }

        //switch (type)
        //{
        //    case Type.STIMULANT:
        //    case Type.NORMAL:
        //        to.StimulateChannels(Mathf.Max(0, weight));
        //        break;
        //    case Type.INHIBITOR:
        //        to.InhibitChannels(Mathf.Max(0, weight));
        //        break;
        //}
        to.StimulateChannels(Mathf.Max(0, weight));

        //        to.pullInIons();

        fired = true;
    }

    public void ToFired(bool toFired)
    {
        if (fired && toFired)
        {
            weight += 0.001f;
        }
        else if (toFired)
        {
            weight -= 0.0001f;
        }
        else if (fired)
        {
            weight -= 0.00001f;
        }
        else
        {
            // Nothing
        }
    }

}
