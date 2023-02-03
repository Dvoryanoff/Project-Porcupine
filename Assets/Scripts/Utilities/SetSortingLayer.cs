using UnityEngine;

public class SetSortingLayer : MonoBehaviour {
    [SerializeField] private string  sortingLayerName = "default";
    void Start () {
        GetComponent<Renderer> ().sortingLayerName = sortingLayerName;
    }
}
