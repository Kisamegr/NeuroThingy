using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralLimb : NeuralBodyPart
{
  public HingeJoint2D hinge;
  public NeuralJoint connectedJoint;
  public bool reverse = false;

  public float angle;
  public float rotationSpeed;

  new protected void Awake() {
    base.Awake ();
    hinge = GetComponent<HingeJoint2D> ();

  }

  void Update() {
    if (hinge) {
      angle = hinge.jointAngle % 360 + hinge.referenceAngle;
      if (angle < 0)
        angle += 360;
      
      rotationSpeed = hinge.jointSpeed;
      
    }
  }

  public override void ResetBodyPart() {
    base.Reset ();
    SetMotorSpeed (0);
  }

  public void SetMotorSpeed(int speed) {
    if (hinge) {
      JointMotor2D motor = hinge.motor;
      motor.motorSpeed = speed;
      hinge.motor = motor;  

//      if (speed != 0)
//        hinge.useMotor = true;
//      else
//        hinge.useMotor = false;
//
      connectedJoint.Rotating (speed);
    }
  }

  public float GetMotorSpeed() {
    if (hinge)
      return hinge.motor.motorSpeed;
    return 0;
  }

 
}
