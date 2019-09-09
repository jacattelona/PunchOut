using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HebbianLearning : Decision
{
    public int numInput = 29;
    public int numOutput = 4;
    public int numHidden = 32;
    public int hiddenLayers = 4;

    private Neuron[][] neurons;
    private List<Synapse> synapses;

    public HebbianLearning()
    {
        synapses = new List<Synapse>();
        neurons = new Neuron[2 + hiddenLayers][];
        neurons[0] = new Neuron[numInput];
        neurons[1 + hiddenLayers] = new Neuron[numOutput];
        for (var i = 1; i < 1 + hiddenLayers; i++)
        {
            neurons[i] = new Neuron[numHidden];
        }

        for (var i = 0; i < neurons.Length; i++)
        {
            for (var j = 0; j < neurons[i].Length; j++)
            {
                neurons[i][j] = new Neuron();
            }
        }

        System.Random r = new System.Random();

        for (var i = 0; i < neurons.Length - 1; i++)
        {
            for (var j = 0; j < neurons[i].Length; j++)
            {
                for (var k = 0; k < neurons[i + 1].Length; k++)
                {
                    if (r.NextDouble() > 0.5)
                    {
                        var synapse = neurons[i][j].AddOutgoingConnection(neurons[i + 1][k]);
                        synapses.Add(synapse);
                    }
                }
            }
        }

    }

    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        foreach (Synapse synapse in synapses)
        {
            if (synapse.fired)
            {
                synapse.weight += 0.1f * reward;
            }
            else
            {
                synapse.weight -= 0.1f * reward;
            }
        }

        for (var i = 0; i < vectorObs.Count; i++)
        {
            neurons[0][i].StimulateChannels(vectorObs[i]);
            neurons[0][i].PullInIons();
        }

        for (var i = 1; i < neurons.Length - 1; i++)
        {
            foreach (Neuron n in neurons[i])
            {
                n.PullInIons();
            }
        }

        bool[] output = new bool[numOutput];
        for (var i = 0; i < neurons[neurons.Length - 1].Length; i++)
        {
            output[i] = neurons[neurons.Length - 1][i].PullInIons();
        }

        float punchState = 0f;
        float dodgeState = 0f;

        if (output[0] && !output[1])
        {
            punchState = 1f;
        }
        else if (!output[0] && output[1])
        {
            punchState = 2f;
        }

        if (output[2] && !output[3])
        {
            dodgeState = 1f;
        }
        else if (!output[2] && output[3])
        {
            dodgeState = 2f;
        }



        return new float[] { dodgeState, punchState };
    }


    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }


}
