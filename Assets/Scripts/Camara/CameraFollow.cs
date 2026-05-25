using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Movimiento")]
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Offset de cámara")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -8f);

    [Header("Bloqueo de ejes")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private bool followZ = false;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;

        Vector3 currentPosition = transform.position;

        Vector3 finalPosition = new Vector3(
            followX ? desiredPosition.x : currentPosition.x,
            followY ? desiredPosition.y : currentPosition.y,
            followZ ? desiredPosition.z : currentPosition.z
        );

        transform.position = Vector3.Lerp(
            currentPosition,
            finalPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}