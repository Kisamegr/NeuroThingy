using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NeuralJoint : NeuralBodyPart
{
  public bool touchingGround = false;

  public Color rightRotationColor;
  public Color leftRotationColor;


  CircleCollider2D circleCollider;

  // Use this for initialization
  new protected void Awake() {
    base.Awake ();

    circleCollider = GetComponent<CircleCollider2D> ();
  }

  public float DistanceFromGround() {
    return body.position.y - circleCollider.bounds.extents.y;
  }

  void OnCollisionEnter2D(Collision2D coll) {
    if (coll.gameObject.tag == "Ground")
      touchingGround = true;
  }

  void OnCollisionExit2D(Collision2D coll) {
    if (coll.gameObject.tag == "Ground")
      touchingGround = false;

  }

  public void Rotating(int direction) {
    Color setColor;
    if (direction > 0)
      setColor = rightRotationColor;
    else if (direction < 0)
      setColor = leftRotationColor;
    else
      setColor = originColor;

    setColor.a = opacity;
    spriteRenderer.color = setColor; 
  }


  public override void ResetBodyPart() {
    body.velocity = Vector2.zero;
    body.angularVelocity = 0;
    body.position = Vector2.zero;
    body.rotation = 0;

    base.Reset ();
  }

}
