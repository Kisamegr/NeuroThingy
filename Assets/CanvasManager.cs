using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml.XPath;
using UnityEditor.VersionControl;

public class CanvasManager : MonoBehaviour
{
  [Header ("UI Stuff")]
  public CanvasGroup topPanel;
  public CanvasGroup bottomPanel;
  public CanvasGroup sidePanel;
  public Text speedSliderText;
  public Text startButtonText;
  public Text lifetimeText;
  public Text generationText;
  public Plot totalFitPlot;
  public Plot bestWorstPlot;
  public Text fps;
  public Toggle bottomToggle;
  public GameObject genPopPrefab;
  public GameObject fitnessPanel;

  
  [Header ("Genetic Options")]
  public InputField lifeTime;
  public InputField population;
  public InputField mutationProb;
  public InputField mutationTimes;
  public InputField timestep;
  public InputField fitDistance;
  public InputField fitSpeed;

  [Header ("Neural Network Options")]
  public InputField hiddenLayerNum;
  public InputField muscleStr;
  public InputField moveThres;
  public InputField tanhScale;

  SimulationManager simulation;
  List<GameObject> genPopItems;

  // Use this for initialization
  void Awake() {
    simulation = GameObject.Find ("_SIMULATION").GetComponent<SimulationManager> ();
    genPopItems = new List<GameObject> ();
  }
	
  // Update is called once per frame
  void Update() {
//
//    if (Time.time - lastFpsTime > 0.5) {
//      fps.text = "FPS: " + Math.Round (1 / Time.deltaTime);
//      lastFpsTime = Time.time;
//    }
		
    if (simulation.running) {
      lifetimeText.text = "Lifetime: " + Mathf.RoundToInt (simulation.creatureLifetime - simulation.lifePassed).ToString ();
      generationText.text = "Generation: #" + (simulation.currentGeneration + 1);

    }
  }

  public void StartButtonPressed() {
    
    if (!simulation.running) {

      // Simulation Options
      simulation.ChangePopulation (Convert.ToInt32 (population.text));
      simulation.creatureLifetime = Convert.ToInt32 (lifeTime.text);
      simulation.mutationProb = Mathf.Clamp ((float)Convert.ToDouble (mutationProb.text), 0f, 1f);
      simulation.maxMutations = Convert.ToInt32 (mutationTimes.text);
      simulation.timeStep = (float)Convert.ToDouble (timestep.text);
      simulation.fitDistanceWeight = (float)Convert.ToDouble (fitDistance.text);
      simulation.fitSpeedWeight = (float)Convert.ToDouble (fitSpeed.text);

      // Neural Options
      simulation.hiddenLayerNum = Convert.ToInt32 (hiddenLayerNum.text);
      simulation.muscleStrength = Convert.ToInt32 (muscleStr.text);
      simulation.moveThreshold = (float)Convert.ToDouble (moveThres.text);
      simulation.tanhScaling = (float)Convert.ToDouble (tanhScale.text);


      totalFitPlot.ResetPlot ();
      bestWorstPlot.ResetPlot ();

      startButtonText.text = "STOP";
      ToggleBottomPanel (false);

      StartCoroutine (simulation.StartGeneration (true));

    } else {
      simulation.ResetSimulation ();
      startButtonText.text = "START";

      ToggleBottomPanel (true);

      //
    }
  }

  public void ChangeTimeScale(Slider slider) {
    Time.timeScale = slider.value / 2;
    speedSliderText.text = "x" + slider.value / 2;

    if (Time.timeScale < 1)
      Time.fixedDeltaTime = Time.timeScale * 0.02f;
    else
      Time.fixedDeltaTime = 0.02f;
    //    Time.fixedDeltaTime = 0.02f * Time.timeScale;
    //    Debug.Log (Time.fixedDeltaTime);
  }

  public void UpdatePlots() {
    float[] totalFit = new float[1];
    totalFit [0] = simulation.genTotalFitness;
    float[] bestWorstFit = new float[3];
    bestWorstFit [0] = simulation.genBestFitness;
    bestWorstFit [1] = simulation.genWorstFitness;
    bestWorstFit [2] = simulation.genTotalFitness / simulation.populationNumber;

    totalFitPlot.UpdatePlot (totalFit);
    bestWorstPlot.UpdatePlot (bestWorstFit);
  }

  public void UpdateSidePanel() {
    if (genPopItems.Count > 0) {
      foreach (GameObject obj in genPopItems)
        Destroy (obj);
      genPopItems.Clear ();
    }

    foreach (KeyValuePair<int,int> pair in simulation.popPerGen) {
      GameObject item = Instantiate (genPopPrefab);
      item.transform.SetParent (sidePanel.transform, false);
      item.GetComponent<GenPopItem> ().SetValues (pair.Key, pair.Value);
      genPopItems.Add (item);
    }
  }

  void ToggleBottomPanel(bool state) {

    if (!state) {
      bottomPanel.alpha = 0;
      bottomPanel.interactable = false;
      bottomToggle.isOn = false; 
    } else {
      bottomPanel.alpha = 1;
      if (!simulation.running)
        bottomPanel.interactable = true;
      bottomToggle.isOn = true; 
    }
  }

  public void onTogglePressed(bool state) {
    ToggleBottomPanel (state);
  }

  public void onFitnessPanelToggled() {
    fitnessPanel.SetActive (!fitnessPanel.activeSelf);
  }
}
