using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Plot : MonoBehaviour
{

  public LineRenderer[] lines;
  public Text[] highest;
  public Text[] current;

  public float[] mins;
  public float[] maxes;
  public List<float>[] values;

  private String[] highestDefaults;
  private String[] currentDefaults;


  // Use this for initialization
  void Awake() {
    values = new List<float>[lines.Length];
    for (int i = 0; i < lines.Length; i++)
      values [i] = new List<float> ();

    if (highest.Length > 0) {
      highestDefaults = new String[highest.Length];
      for (int i = 0; i < lines.Length; i++) {
        if (highest [i] != null)
          highestDefaults [i] = highest [i].text;   
      }
    }

    if (current.Length > 0) {
      currentDefaults = new String[current.Length];
      for (int i = 0; i < lines.Length; i++) {
        if (current [i] != null)
          currentDefaults [i] = current [i].text;     
      }
    }

    mins = Enumerable.Repeat (10000f, lines.Length).ToArray ();
    maxes = Enumerable.Repeat (-10000f, lines.Length).ToArray (); 
  }

  public void UpdatePlot(float[] newValues) {
//    if (values.Count == 0)
//      return;
    
    Rect plotSize = lines [0].GetComponent<RectTransform> ().rect;
    int valueCount = values [0].Count;
    float xSpacing = valueCount > 1 ? plotSize.width / (valueCount) : plotSize.width;

    int newValueCount = Mathf.Max (valueCount + 1, 2);
//    Vector3[] points = new Vector3[pointCount];

    for (int i = 0; i < lines.Length; i++) {
      if (newValues [i] > maxes [i])
        maxes [i] = newValues [i];
      if (newValues [i] < mins [i])
        mins [i] = newValues [i]; 
    }

    float min = mins.Min ();
    float max = maxes.Max ();
    float scale = plotSize.height / (Math.Abs (min) + Math.Abs (max));
     
    float offset = plotSize.height / 2;

//    foreach (float v in values) {
//      if (min > v)
//        min = v;
//      if (max < v)
//        max = v;
//    }
    for (int i = 0; i < lines.Length; i++) {
      Vector3[] positions = new Vector3[newValueCount];

      for (int j = 0; j < newValueCount; j++) {
        if (j < valueCount)
          positions [j].Set (j * xSpacing, (values [i] [j] - min) * scale - offset, 0);
        else
          positions [j].Set (j * xSpacing, (newValues [i] - min) * scale - offset, 0);
      }
        
      values [i].Add (newValues [i]);
      lines [i].positionCount = newValueCount;
      lines [i].SetPositions (positions);
    }

    Vector2 pos;

    if (highest.Length > 0) {
      for (int i = 0; i < lines.Length; i++) {
        if (highest [i] != null) {
          highest [i].text = Convert.ToString (Math.Round (maxes [i], 2));
          pos = highest [i].rectTransform.anchoredPosition;
          pos.y = (maxes [i] - min) * scale - offset;
          highest [i].rectTransform.anchoredPosition = pos;   
        }
      }
    }

    if (current.Length > 0) {
      for (int i = 0; i < lines.Length; i++) {
        if (current [i] != null) {
          current [i].text = Convert.ToString (Math.Round (newValues [i], 2));
          pos = current [i].rectTransform.anchoredPosition;
          pos.y = (newValues [i] - min) * scale - offset;
          current [i].rectTransform.anchoredPosition = pos;
        }
      }
    }
      
  }

  public void ResetPlot() {
    mins = Enumerable.Repeat (10000f, lines.Length).ToArray ();
    maxes = Enumerable.Repeat (-10000f, lines.Length).ToArray ();

    foreach (LineRenderer line in lines) {
      line.positionCount = 0;
//      line.SetPositions (null);
    }

    if (highest.Length > 0) {
      for (int i = 0; i < lines.Length; i++) {
        if (highest [i] != null)
          highest [i].text = highestDefaults [i];        
      }
    }

    if (current.Length > 0) {
      for (int i = 0; i < lines.Length; i++) {
        if (current [i] != null)
          current [i].text = currentDefaults [i];      
      }
    }

    foreach (List<float> value in values) {
      value.Clear ();
    }
  }
}
