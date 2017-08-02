using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection.Emit;

public class SimulationManager : MonoBehaviour
{
  [Header ("Gameobjects")]
  public Transform cameraTransform;
  public GameObject creaturePrefab;
  public CanvasManager ui;


  [Header ("Genetic Options")]
  public bool running = false;
  public int currentGeneration = 0;
  public int populationNumber;
  public float creatureLifetime;
  public float mutationProb;
  public int maxMutations;
  public float timeStep = 0.1f;
  public float fitDistanceWeight;
  public float fitSpeedWeight;

  [Header ("Neural Network Options")]
  public int hiddenLayerNum;
  public int muscleStrength;
  public float moveThreshold;
  public float tanhScaling;

 
  [Header ("Misc")]
  public float lifePassed;
  public float genTotalFitness;
  public float genBestFitness;
  public float genWorstFitness;
  public SortedDictionary<int, int> popPerGen;
  //  public List<float> totalFitPerGen;

  private List<Creature> spawnedCreatures;

  private float beginTime;

  private int halfPopulation;
  private int quarterPopulation;
  private float lastUpdateTime;

  private bool thinking = false;
  private int thinkTimes;

  // Use this for initialization
  void Awake() {
    UnityEngine.Random.InitState (System.DateTime.Now.Millisecond);
    spawnedCreatures = new List<Creature> ();
//    totalFitPerGen = new List<float> ();

    lastUpdateTime = Time.time;
    popPerGen = new SortedDictionary<int, int> ();

  }
	
  // Update is called once per frame
  void Update() {

    if (running) {
      /* if (Time.time - lastUpdateTime > timeStep) {
        if (!thinking) {
          thinking = true;
          CreatureThinking ();      
          thinkTimes++;
        } else
          Debug.Log ("GOING TOO FAST MAN ");

        lastUpdateTime = Time.time;
      }*/

      lifePassed = Time.time - beginTime;

      if (lifePassed > creatureLifetime) {
        running = false;
        CancelInvoke ();
        StartCoroutine (EvaluateGeneration ());
        return;
      }
    }
  }

  public IEnumerator StartGeneration(bool firstTime) {
    Debug.Log ("Starting Generation " + currentGeneration);
    if (firstTime) {
      for (int i = 0; i < populationNumber; i++) {
        Creature newCreature = createCreature (0);
        newCreature.neuralNet.InitializeRandomNet ();

        spawnedCreatures.Add (newCreature);
      }
    }

    // Find how many creatures per generation exist
    popPerGen.Clear ();
    foreach (Creature c in spawnedCreatures) {
      if (!popPerGen.ContainsKey (c.generationCreated))
        popPerGen.Add (c.generationCreated, 1);
      else
        popPerGen [c.generationCreated] = popPerGen [c.generationCreated] + 1;
    }

    ui.UpdateSidePanel ();

    cameraTransform.GetComponent<FollowCreature> ().Reset ();
    running = true;
    thinkTimes = 0;
    beginTime = Time.time;
    InvokeRepeating ("CreatureThinking", timeStep, timeStep);
    yield break;
  }

  private void CreatureThinking() {
    // Update the creatures...
//    float timePassed = 0;
    thinkTimes++;
    float startTime = Time.realtimeSinceStartup;
    float frameTime = startTime;

    foreach (Creature creature in spawnedCreatures) {
//      timePassed = Time.realtimeSinceStartup - frameTime;
//      if (timePassed > Time.deltaTime) {
//        frameTime = Time.realtimeSinceStartup;
////        Debug.Log ("EXCEDING THINKING IN A FRAME");
//
//        yield return null;
//      }
//
      creature.Think ();
    }

//    Debug.Log ("Thinking time: " + (Time.realtimeSinceStartup - startTime) + "s");

    thinking = false;
//    yield break;
  }

  private IEnumerator EvaluateGeneration() {
//    Debug.Log ("Evaluating Generation...");

    // Sort the spawned creatures based on their fitness value
//    Debug.Log ("Sorting Spawned Creatures...");
    spawnedCreatures.Sort (
      delegate(Creature p1, Creature p2) {
        return p2.fitness.CompareTo (p1.fitness); 
      }
    );

    genTotalFitness = 0;
    foreach (Creature c in spawnedCreatures)
      genTotalFitness += c.fitness;
    

    genBestFitness = spawnedCreatures [0].fitness;
    genWorstFitness = spawnedCreatures [populationNumber - 1].fitness;

    // Kill the last half/worst creatures
    for (int i = halfPopulation; i < populationNumber; i++)
      Destroy (spawnedCreatures [i].gameObject);
    spawnedCreatures.RemoveRange (halfPopulation, populationNumber - halfPopulation);

    // Crossbreed
    yield return StartCoroutine (Crossbreed ());

    // Reset the creatures to their original position and zero fitness
//    Debug.Log ("Reseting creatures...");
    for (int i = 0; i < halfPopulation; i++) {
      spawnedCreatures [i].ResetCreature ();
    }

    Debug.Log ("Generation " + currentGeneration + " finished. Total fitness: " + genTotalFitness);
    Debug.Log ("Think Times: " + thinkTimes);
    Debug.Log ("-=-==-=-=-=-=-=-=-=--==--=-=-==--=-=-=--");

    ui.UpdatePlots ();
    currentGeneration++;
      
    StartCoroutine (StartGeneration (false));
    yield break;
  }

  private IEnumerator Crossbreed() {
//    Debug.Log ("Crossbreeding...");
    float totalFitness = 0;
    List<float> fitRoulette = new List<float> ();

    // Calculate the total fitness
    foreach (Creature c in spawnedCreatures)
      totalFitness += c.fitness;
//    totalFitPerGen.Add (totalFitness);

    // Calculate the fitness roulette based on each creature's fitness
    fitRoulette.Add (spawnedCreatures [0].fitness / totalFitness);
    for (int i = 1; i < halfPopulation; i++)
      fitRoulette.Add (fitRoulette [i - 1] + spawnedCreatures [i].fitness / totalFitness);

    // Generate children and mutate them
    int pIndex1, pIndex2;
    int totalConnections = 0;
    NeuralNet tempNet = spawnedCreatures [0].neuralNet;
    for (int l = 1; l < tempNet.N.Length; l++)
      totalConnections += tempNet.N [l] * tempNet.N [l - 1];

    for (int i = 0; i < quarterPopulation; i++) {
      float r1, r2;

      r1 = UnityEngine.Random.value;
      pIndex1 = fitRoulette.BinarySearch (r1);

      do {
        r2 = UnityEngine.Random.value;
        pIndex2 = fitRoulette.BinarySearch (r2);
      } while (pIndex2 == pIndex1);


      if (pIndex1 < 0)
        pIndex1 = Mathf.Clamp (0, ~pIndex1 - 1, fitRoulette.Count);
      if (pIndex2 < 0)
        pIndex2 = Mathf.Clamp (0, ~pIndex2 - 1, fitRoulette.Count);

      int counter = 0;
//      int pr = UnityEngine.Random.Range (0, totalConnections);
      int neuron = UnityEngine.Random.Range (0, tempNet.N [1]);

      List<Creature> children = new List<Creature> ();

      for (int c = 0; c < 2; c++) {

        if (spawnedCreatures.Count == populationNumber)
          break;

        children.Add (createCreature (currentGeneration + 1));

        for (int l = 1; l < tempNet.N.Length; l++) {
          for (int x = 0; x < tempNet.N [l]; x++) {
            for (int y = 0; y < tempNet.N [l - 1]; y++) {
              if (l == 1 && x == neuron)
                children [c].neuralNet.w [l - 1] [x] [y] = spawnedCreatures [c == 0 ? pIndex2 : pIndex1].neuralNet.w [l - 1] [x] [y];
              else
                children [c].neuralNet.w [l - 1] [x] [y] = spawnedCreatures [c == 0 ? pIndex1 : pIndex2].neuralNet.w [l - 1] [x] [y];
//              if (counter < pr)
//                children [c].neuralNet.w [l - 1] [x] [y] = spawnedCreatures [c == 0 ? pIndex1 : pIndex2].neuralNet.w [l - 1] [x] [y];
//              else
//                children [c].neuralNet.w [l - 1] [x] [y] = spawnedCreatures [c == 0 ? pIndex2 : pIndex1].neuralNet.w [l - 1] [x] [y];
              
              counter++;
            }
          }
        }


        // MUtate child
        float mr = UnityEngine.Random.value;
        if (mr < mutationProb) {
          int mutationTimes = UnityEngine.Random.Range (0, maxMutations) + 1;

          for (int m = 0; m < mutationTimes; m++) {
            int mNum = UnityEngine.Random.Range (0, 4);
            int con = UnityEngine.Random.Range (0, totalConnections);
            float oldWeight = children [c].neuralNet.GetWeightAt (con);

            switch (mNum) {
            case 0:
              children [c].neuralNet.SetWeightAt (con, UnityEngine.Random.Range (-1f, 1f));
              break;
            case 1:
              children [c].neuralNet.SetWeightAt (con, oldWeight * UnityEngine.Random.Range (0.5f, 1.5f));
              break;
            case 2:
              float value = UnityEngine.Random.value;
              if (oldWeight <= 0)
                children [c].neuralNet.SetWeightAt (con, oldWeight + value);
              else
                children [c].neuralNet.SetWeightAt (con, oldWeight - value);
              break;
            case 3: // Change the weight sign
              children [c].neuralNet.SetWeightAt (con, (-1) * oldWeight);
              break;
            }
          }
          children [c].mutated = true;
        }

        spawnedCreatures.Add (children [c]);
      
      }

    }


    yield break;
  }

  public void ChangePopulation(int value) {
    populationNumber = value;
    halfPopulation = Mathf.CeilToInt (populationNumber / 2f);
    quarterPopulation = Mathf.CeilToInt (populationNumber / 4f);
  }


  private Creature createCreature(int genNum) {
    Creature creature = Instantiate (creaturePrefab, creaturePrefab.transform.position, Quaternion.identity).GetComponent<Creature> ();
    creature.generationCreated = genNum;
    creature.neuralNet.N [1] = hiddenLayerNum;
    creature.neuralNet.moveThreshold = moveThreshold;
    creature.neuralNet.ht = tanhScaling;
    creature.muscleStrength = muscleStrength;

    creature.neuralNet.CreateNet ();

    return creature;
  }

  public void ResetSimulation() {
    running = false;
    StopAllCoroutines ();
    CancelInvoke ();
    currentGeneration = 0;

    foreach (Creature creature in spawnedCreatures) {
      Destroy (creature.gameObject);
    }

    spawnedCreatures.Clear ();
//    totalFitPerGen.Clear ();
  }
    
}
