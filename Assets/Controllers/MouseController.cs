using UnityEngine;

public class MouseController : MonoBehaviour {

    [SerializeField] private GameObject circleCursor;

    private Vector3 lastFramePosition;

    Vector3 dragStartPosition;

    void Update() {

        Vector3 currentFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentFramePosition.z = 0;

        Tile tileUnderMouse = GetTileAtWorldCoord(currentFramePosition);

        if (tileUnderMouse != null) {
            circleCursor.SetActive(true);

            Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
            circleCursor.transform.position = cursorPosition;
        } else { circleCursor.SetActive(false); }

        // Start Drag

        if (Input.GetMouseButtonDown(0)) {
            dragStartPosition = currentFramePosition;
        }

        // End Drag

        if (Input.GetMouseButtonUp(0)) {
            int startX = Mathf.FloorToInt(dragStartPosition.x);
            int endX = Mathf.FloorToInt(currentFramePosition.x);

            if (endX < startX) {
                int tmp = endX;
                endX = startX;
                startX = tmp;

            }

            int startY = Mathf.FloorToInt(dragStartPosition.y);
            int endY = Mathf.FloorToInt(currentFramePosition.y);

            if (endY < startY) {
                int tmp = endY;
                endY = startY;
                startY = tmp;

            }

            for (int x = startX; x <= endX; x++) {
                for (int y = startY; y <= endY; y++) {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null) {
                        t.Type = Tile.TileType.Floor;
                    }

                }

            }
            //if (tileUnderMouse != null) {
            //    if (tileUnderMouse.Type == Tile.TileType.Empty) {
            //        tileUnderMouse.Type = Tile.TileType.Floor;
            //    } else tileUnderMouse.Type = Tile.TileType.Empty;
            //}

        }

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
            Vector3 deltaFramePosition = lastFramePosition - Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
            Camera.main.transform.Translate(deltaFramePosition);
        }

        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentFramePosition.z = 0;

    }

    Tile GetTileAtWorldCoord(Vector3 coord) {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return WorldController.Instance.World.GetTileAt(x, y);
    }
}
