using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class Ground : MonoBehaviour
{

  public Transform groundCollider;
  public GameObject groundPrefab;
  public int groundNumber;


  // Use this for initialization
  void Awake() {
    Vector3 spriteSize = groundPrefab.GetComponent<SpriteRenderer> ().bounds.size;
    transform.position = new Vector3 (transform.position.x, -spriteSize.y / 2, transform.position.z);

    groundCollider.position = new Vector3 (groundNumber / 2f * spriteSize.x + transform.position.x - spriteSize.x / 2, groundCollider.position.y, groundCollider.position.z);
    groundCollider.localScale = new Vector3 (groundNumber, groundCollider.localScale.y, groundCollider.localScale.z);

    for (int i = 0; i < groundNumber; i++) {
      GameObject ground = GameObject.Instantiate (groundPrefab, new Vector3 (transform.position.x + spriteSize.x * i, transform.position.y, transform.position.z), Quaternion.identity);
      ground.transform.parent = transform;
    }

  }
	
  // Update is called once per frame
  void Update() {
		
  }
}
