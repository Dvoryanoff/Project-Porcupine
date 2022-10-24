using UnityEngine;

[ExecuteInEditMode]
public class AutomaticVerticalSize : MonoBehaviour {

    [SerializeField] private float childHeight = 28f;

    private void Start() {
        AdjustSise();
    }

    private void Update() {
        AdjustSise();
    }

    private void AdjustSise() {
        Vector2 size = this.GetComponent<RectTransform>().sizeDelta;
        size.y = this.transform.childCount * childHeight;
        this.GetComponent<RectTransform>().sizeDelta = size;
    }
}
