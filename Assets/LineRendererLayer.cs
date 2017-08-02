using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
[ExecuteInEditMode]

public class LineRendererLayer : MonoBehaviour
{
  public string sortingLayer;

  private Renderer getMeshRenderer() {
    return gameObject.GetComponent<Renderer> ();
  }

  void Update() {
    if (getMeshRenderer ().sortingLayerName != sortingLayer && sortingLayer != "") {
      //Debug.Log("Forcing sorting layer: "+sortingLayer);
      getMeshRenderer ().sortingLayerName = sortingLayer;
    }
  }
}