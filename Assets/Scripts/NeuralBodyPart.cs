using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NeuralBodyPart : MonoBehaviour
{

  public SpriteRenderer spriteRenderer;
  public Rigidbody2D body;


  Vector3 originPosition;
  Quaternion originRotation;
  protected Color originColor;
  protected float opacity;

  // Use this for initialization
  protected void Awake() {
    spriteRenderer = GetComponent<SpriteRenderer> ();
    body = GetComponent<Rigidbody2D> ();

    originPosition = transform.position;
    originRotation = transform.rotation;
    originColor = spriteRenderer.color;
    opacity = originColor.a;
  }

 
  public void SetOpaque() {
    opacity = 0.7f;
    Color opaqueColor = spriteRenderer.color;
    opaqueColor.a = opacity;
    spriteRenderer.color = opaqueColor;
  }

  protected void Reset() {
    transform.position = originPosition;
    transform.rotation = originRotation;
    spriteRenderer.color = originColor;
    opacity = originColor.a;
  }

  public abstract void ResetBodyPart();
}
