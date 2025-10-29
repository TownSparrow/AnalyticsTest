using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera cam;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        // Test with mouse in editor + handling touches on the device
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
        {
            Vector3 screenPos;
            if (Input.touchCount > 0)
                screenPos = Input.touches[0].position;
            else
                screenPos = Input.mousePosition;

            Vector2 worldPoint = cam.ScreenToWorldPoint(screenPos);

            // Instead of Raycast, use OverlapPoint for 2D collider by touch
            Collider2D collider = Physics2D.OverlapPoint(worldPoint);
            if (collider != null && collider.TryGetComponent<TapTarget>(out var tapTarget))
            {
                tapTarget.OnTapped();
            }
            else
            {
                Debug.Log("No collider tapped at " + worldPoint);
            }
        }
    }
}
