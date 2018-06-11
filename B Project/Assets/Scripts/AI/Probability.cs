using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probability : MonoBehaviour {

    public float[] probs;
    private int[] counts;
    [Range(0f,1f)]
    public float weight = 0.5f;

    void Start () {
	}
	
	void Update () {
	}
    
    //Updates probability of the value at index k and returns it new value
    //e.g 0.50 weight means reduce by 50%
    //This is done by solving the equation weight*x_1 + x_2 + ..... +x_n = 1 for x_1
    public float updateProbs(int k) {
        counts[k]++;
        float s = 0.0f;
        float q1 = 0; //The probability being modified

        //Calculate the amount to decrease the probability
        for (int i = 0; i < k; i++) {
            s += probs[i];
            q1 += Mathf.Pow((1 / weight), counts[k] - counts[i]);
        }
        for (int i = k + 1; i < probs.Length; i++) {
            s += probs[i];
            q1 += Mathf.Pow((1 / weight), counts[k] - counts[i]);
        }

        float q2 = 1 / (q1 + 1);

        float q3 = (1 - s - q2) / (probs.Length - 1);

        //Increase all of the other probabilities so that their sum = 1
        for (int i = 0; i < k; i++) {
            probs[i] += q3;
        }
        for (int i = k+1; i < probs.Length; i++) {
            probs[i] += q3;
        }

        probs[k] = q2;
        return q2;
    }

    public void setupProbs(int l) {
        probs = new float[l];
        counts = new int[l];

        float p = 1 / l;

        for (int i = 0; i < l; i++) {
            probs[i] = p;
            counts[i] = 0;
        }
    }

    public int pickIndex(float[] distribution)
    {
        float x = Random.Range(0f, 1.0f);
        float s = 0;
        for(int i = 0; i < distribution.Length; i++)
        {
            s += distribution[i];
            if(s >= x)
            {
                return i;
            }
        }
        
        return 0;
    }

    public void normalizeDistribution(float[] distribution)
    {
        float s = 0;

        for (int i = 0; i < distribution.Length; i++)
        {
            s += distribution[i];
        }

        for(int i = 0; i < distribution.Length; i++)
        {
            distribution[i] /= s;
        }
    }

    private void logDistribution() {
        foreach(var f in probs){
            Debug.Log(f);
        }
    }


}
