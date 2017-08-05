using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {
  public NeuralJoint[] joints;
  public NeuralLimb[] limbs;

  public float fitness;
  public Vector2 centroid;
  public float medianSpeed;

  public int generationCreated;
  public bool mutated;
  public int muscleStrength;

  public NeuralNet neuralNet;

  private float speedSum;
  private int speedCounter;

  private SimulationManager simulation;

  private void Awake() {
    simulation = GameObject.Find("_SIMULATION").GetComponent<SimulationManager>();
  }

  void FixedUpdate() {
    Vector2 posSum = Vector2.zero;
    foreach (NeuralJoint joint in joints)
      posSum += joint.body.position;

    centroid = posSum / joints.Length;

    float limbSpeedSum = 0;
    foreach (NeuralLimb limb in limbs) 
      limbSpeedSum += limb.body.velocity.x;

    speedSum += limbSpeedSum / limbs.Length;
    speedCounter++;
    medianSpeed = speedSum / speedCounter;
  }

  public void Think() {
    neuralNet.UpdateNeuralNet(ref joints,ref limbs, muscleStrength);

    fitness = simulation.fitDistanceWeight*centroid.x + simulation.fitSpeedWeight*medianSpeed;
  }

  public void ResetCreature() {

    foreach (NeuralJoint joint in joints)
      joint.ResetBodyPart();

    foreach (NeuralLimb limb in limbs)
      limb.ResetBodyPart();
    
    
    
    
    fitness = 0;
    centroid = Vector2.zero;
    speedSum = 0;
    speedCounter = 0;
    medianSpeed = 0;
  }

}