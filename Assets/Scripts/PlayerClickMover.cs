using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class PlayerClickMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.2f;
    public float rotationSpeed = 12f;
    public float stopDistance = 0.05f;
    public Vector2 roomBounds = new Vector2(4.6f, 3.1f);

    [Header("Scene References")]
    public Camera sceneCamera;
    public Transform targetMarker;

    private Vector3 targetPosition;
    private bool hasTarget;

    private void Awake()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }

        targetPosition = transform.position;
        SetMarkerVisible(false);
    }

    private void Update()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }

        HandleClickInput();
        MoveTowardTarget();
    }

    public void MoveTo(Vector3 worldPosition)
    {
        worldPosition.y = transform.position.y;
        worldPosition.x = Mathf.Clamp(worldPosition.x, -roomBounds.x, roomBounds.x);
        worldPosition.z = Mathf.Clamp(worldPosition.z, -roomBounds.y, roomBounds.y);

        targetPosition = worldPosition;
        hasTarget = true;

        if (targetMarker != null)
        {
            targetMarker.position = new Vector3(targetPosition.x, 0.03f, targetPosition.z);
            SetMarkerVisible(true);
        }
    }

    private void HandleClickInput()
    {
        if (!Input.GetMouseButtonDown(0) || sceneCamera == null)
        {
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f) &&
            hit.collider.GetComponentInParent<Interactable>() != null)
        {
            return;
        }

        Plane floorPlane = new Plane(Vector3.up, Vector3.zero);
        if (floorPlane.Raycast(ray, out float distance))
        {
            MoveTo(ray.GetPoint(distance));
        }
    }

    private void MoveTowardTarget()
    {
        if (!hasTarget)
        {
            return;
        }

        Vector3 toTarget = targetPosition - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude <= stopDistance)
        {
            hasTarget = false;
            SetMarkerVisible(false);
            return;
        }

        Vector3 direction = toTarget.normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void SetMarkerVisible(bool visible)
    {
        if (targetMarker != null)
        {
            targetMarker.gameObject.SetActive(visible);
        }
    }
}
