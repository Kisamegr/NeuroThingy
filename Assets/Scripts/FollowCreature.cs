using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCreature : MonoBehaviour
{
  public float yOffset = 2;
  public Creature target;
  public List<Creature> targets;
  public float cameraDeltaUpdate = 0.025f;
  public float followSpeed;
  public float xLimit;

  private SimulationManager simulation;
  private Vector3 originalPosition;
  private Vector3 newPosition;

  private float lastUpdateTime;
  // Use this for initialization
  void Awake() {
    targets = new List<Creature> ();
    newPosition = new Vector3 ();
    simulation = GameObject.Find ("_SIMULATION").GetComponent<SimulationManager> ();
    originalPosition = transform.position;
    lastUpdateTime = Time.time;
  }

  // Update is called once per frame
  void Update() {
    if (Time.time - lastUpdateTime > cameraDeltaUpdate) {
      if (simulation.running && targets.Count > 0) {
        foreach (Creature c in targets) {
          Vector2 frontJointPosition = c.joints [c.joints.Length - 1].body.position;
          if (target == null || frontJointPosition.x > target.centroid.x) {
            target = c;
            break;
          }
        }

      }

      lastUpdateTime = Time.time;
    }

    if (target) {
      newPosition.Set (target.centroid.x, target.centroid.y + yOffset, transform.position.z);
      transform.position = Vector3.Lerp (transform.position, newPosition, followSpeed * Time.deltaTime);
    }
  }

  public void Reset() {
    target = null;
    targets.Clear ();
    transform.position = originalPosition;
    lastUpdateTime = Time.time;
  }
}
