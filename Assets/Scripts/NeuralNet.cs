using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NeuralNet : MonoBehaviour
{


  int layerNumber;
  public int[] N;


  // The output value of each neuron, in each layer
  public float[][] a;
  // The Weight values of the neurons synapses of this layer, with each of the previous one
  public float[][][] w;
  // The biases weight values for each neuron, in each layer
  float[][] bias;

  public bool logistic = true;


  public double hl = 1;
  //0.2375;
  public double ht = 0.5;
  //0.05;
  // Use this for initialization


  public float moveThreshold = 0f;

  public float[] input;
  public float[] output;


  public void UpdateNeuralNet(ref NeuralJoint[] joints, ref NeuralLimb[] limbs, int muscleStrength) {
    
    // *************
    // Set the input
    // *************

    int i = 0, j = 0;
    while (i < joints.Length) {
//      a [0] [i] = joints [i].touchingGround ? 1 : -1;
      a [0] [i] = joints [i].DistanceFromGround ();
      input [i] = a [0] [i];
      i++;
    }

    foreach (NeuralLimb limb in limbs) {
      if (limb.hinge) {
//        a [0] [j + i] = limb.GetMotorSpeed () > moveThreshold ? 1 : (limb.GetMotorSpeed () < -moveThreshold ? -1 : 0);
//        input [j + i] = a [0] [i + j];       
//        j++;

        a [0] [j + i] = limb.angle < 180 ? limb.angle / 180f : (limb.angle - 360) / 180f;
        input [j + i] = a [0] [i + j];       
        j++;
      }
    }



    a [0] [N [0] - 1] = a [layerNumber - 1] [N [layerNumber - 1] - 1]; // Memory
    input [N [0] - 1] = a [0] [i + j];


    // *************
    // Pass through the network
    // *************

    NeuralNetPass ();


    // *************
    // Get the output
    // *************

    i = 0;
    foreach (NeuralLimb limb in limbs) {
      if (limb.hinge) {
//        limb.SetMotorSpeed (a [layerNumber - 1] [i] > moveThreshold ? muscleStrength : (a [layerNumber - 1] [i] < -moveThreshold ? -muscleStrength : 0));
        limb.SetMotorSpeed (Mathf.RoundToInt (a [layerNumber - 1] [i] * muscleStrength));
        output [i] = a [layerNumber - 1] [i];
        i++;
      }
    }
    output [i] = a [layerNumber - 1] [i];
  

  }

  public void InitializeRandomNet() {

    //std::cout << "Initializing Random Deep Net..." << std::endl;


    // Initialize the arrays values
    for (int l = 1; l < layerNumber; l++) { // For each layer...
      for (int i = 0; i < N [l]; i++) { // For each neuron in the l layer...
        bias [l - 1] [i] = 1; // Initialize the bias to -1
        float g = 1;//(float)(2.38 / Math.Sqrt (N [l - 1])); // Calculate the range of the random weights for each layer, based on the number of neurons from the previous layer

        for (int j = 0; j < N [l - 1]; j++) { // For each neuron in the l-1 layer...
          float r = UnityEngine.Random.value * g - g / 2; // Calculate a random value within the g range, centered to 0
          w [l - 1] [i] [j] = r; // Set the weight
          //Debug.Log ("Weight: " + r);
        }
      }
    }
  }



  public void  CreateNet() {
    layerNumber = N.Length;
   
    a = new float[layerNumber][];
    w = new float[layerNumber - 1][][];
    bias = new float[layerNumber - 1][];

    // For each layer
    for (int l = 0; l < layerNumber; l++) {
      a [l] = new float[N [l]]; // Create the output array

      if (l != 0) {
        w [l - 1] = new float[N [l]][]; // Create the weights array
        bias [l - 1] = new float[N [l]]; // Create the bias weights array

        // For each neuron in the l layer
        for (int i = 0; i < N [l]; i++)
          w [l - 1] [i] = new float[N [l - 1]]; // Create each neuron input-weights array
      }

    }

    for (int i = 0; i < N [0]; i++)
      a [0] [i] = UnityEngine.Random.Range (-1f, 1f);

    input = new float[N [0]];
    output = new float[N [layerNumber - 1]];

  }

  void NeuralNetPass() {
    for (int l = 1; l < layerNumber; l++) { // For each layer...
      for (int i = 0; i < N [l]; i++) { // For each neuron in this layer
        float sum = 0;

        for (int j = 0; j < N [l - 1]; j++) // For each neuron in the previous layer
								sum += w [l - 1] [i] [j] * a [l - 1] [j]; // Sum up all the outputs*weights

        //sum += bias [l - 1] [i]; // Add the bias

        if (logistic)
          a [l] [i] = Logistic (sum); // Pass it through the Logistic function
							else
          a [l] [i] = Tanh (sum); // Or pass it through the Tanh function

      }
    }
  }

  float Logistic(float x) {
    return (float)(1 / (1 + Math.Exp (-hl * x)));
  }

  float Tanh(float x) {
    return (float)Math.Tanh (ht * x);
  }

  public float GetWeightAt(int index) {
    int currentIndex = index;
    for (int l = 1; l < N.Length; l++) {
      int layerCons = N [l] * N [l - 1];
      if (currentIndex < layerCons) {
        return w [l - 1] [currentIndex / N [l - 1]] [currentIndex % N [l - 1]];
      } else
        currentIndex -= layerCons;
    }
      
    return -1;
  }

  public void SetWeightAt(int index, float value) {
    int currentIndex = index;
    for (int l = 1; l < N.Length; l++) {
      int layerCons = N [l] * N [l - 1];
      if (currentIndex < layerCons) {
        w [l - 1] [currentIndex / N [l - 1]] [currentIndex % N [l - 1]] = value;
      } else
        currentIndex -= layerCons;
    }
  }

}
