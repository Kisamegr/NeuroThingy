using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GenPopItem : MonoBehaviour
{

  public Text genText;
  public Text popText;

  // Use this for initialization
  public void SetValues(int gen, int pop) {
    genText.text = "G: " + Convert.ToString (gen);
    popText.text = "#" + Convert.ToString (pop);
  }
}
