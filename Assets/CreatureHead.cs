using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHead : MonoBehaviour
{

  public SpriteRenderer parentRenderer;
  public SpriteRenderer[] eyeRenderers;
  private SpriteRenderer thisRenderer;
  // Use this for initialization
  void Start() {
    thisRenderer = GetComponent<SpriteRenderer> ();
  }
	
  // Update is called once per frame
  void Update() {
    thisRenderer.color = parentRenderer.color;
  }
}
