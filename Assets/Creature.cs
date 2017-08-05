using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour{
    public NeuralJoint[] joints;
    public Vector2 centroid;
    public int generationCreated;
    public float fitness;
    public bool mutated;
    public int muscleStrength;

    public NeuralNet neuralNet;

    public void Awake() {
        
    }

    public void Think() {

    }

    public void ResetCreature() {
        
    }

}