using UnityEngine;

public class MouseController : MonoBehaviour {

    [SerializeField] private GameObject circleCursor;

    private Vector3 lastFramePosition;
    void Start() {
        //Application.targetFrameRate = 60;

    }

    // Update is called once per frame
    void Update() {

        Vector3 currentFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentFramePosition.z = 0;

        circleCursor.transform.position = currentFramePosition;

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
            Vector3 deltaFramePosition = lastFramePosition - Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
            Camera.main.transform.Translate(deltaFramePosition);
        }

        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentFramePosition.z = 0;

    }
}
