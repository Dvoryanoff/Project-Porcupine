using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

    [SerializeField] private GameObject circleCursorPrefab;

    private Vector3 lastFramePosition;
    private Vector3 dragStartPosition;
    private Vector3 currentFramePosition;

    private float minCameraZoom = 3f;
    private float maxCameraZoom = 40f;

    List<GameObject> dragPreviewGameobjects;

    private void Start() {
        dragPreviewGameobjects = new List<GameObject>();
        //SimplePool.Preload(circleCursorPrefab, 100);
    }

    void Update() {

        currentFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentFramePosition.z = 0;

        //UpdateCursor();
        UpdateDragging();
        UpdateMainCamera();

        #region
        //if (tileUnderMouse != null) {
        //    if (tileUnderMouse.Type == Tile.TileType.Empty) {
        //        tileUnderMouse.Type = Tile.TileType.Floor;
        //    } else tileUnderMouse.Type = Tile.TileType.Empty;
        //}
        #endregion

        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentFramePosition.z = 0;

    }

    private void UpdateMainCamera() {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
            Vector3 deltaFramePosition = lastFramePosition - Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
            Camera.main.transform.Translate(deltaFramePosition);
        }

        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minCameraZoom, maxCameraZoom);
    }

    private void UpdateDragging() {

        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }
        // Start Drag

        if (Input.GetMouseButtonDown(0)) {
            dragStartPosition = currentFramePosition;
        }

        int startX = Mathf.FloorToInt(dragStartPosition.x);
        int endX = Mathf.FloorToInt(currentFramePosition.x);
        int startY = Mathf.FloorToInt(dragStartPosition.y);
        int endY = Mathf.FloorToInt(currentFramePosition.y);

        if (endX < startX) {
            (startX, endX) = (endX, startX);
        }

        if (endY < startY) {
            (startY, endY) = (endY, startY);
        }

        while (dragPreviewGameobjects.Count > 0) {
            GameObject previewGameObject = dragPreviewGameobjects[0];
            dragPreviewGameobjects.RemoveAt(0);
            SimplePool.Despawn(previewGameObject);

        }
        if (Input.GetMouseButton(0)) {
            for (int x = startX; x <= endX; x++) {
                for (int y = startY; y <= endY; y++) {
                    Tile t = WorldController.Instance.world.GetTileAt(x, y);
                    if (t != null) {
                        GameObject previewGameObject = SimplePool.Spawn(circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        previewGameObject.transform.SetParent(this.transform, true);
                        dragPreviewGameobjects.Add(previewGameObject);
                    }
                }
            }
        }

        // End Drag

        if (Input.GetMouseButtonUp(0)) {

            BuildModeController bmc = GameObject.FindObjectOfType<BuildModeController>();

            // Loop through Tiles.

            for (int x = startX; x <= endX; x++) {
                for (int y = startY; y <= endY; y++) {
                    Tile t = WorldController.Instance.world.GetTileAt(x, y);
                    if (t != null) {
                        bmc.DoBuild(t);
                    }
                }
            }
        }
    }
}