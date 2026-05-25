using System.Collections;
using UnityEngine;

public class MovingPlatformElevator : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float height = 5f;
    [SerializeField] private float speed = 2f;

    [Header("Tiempos de espera")]
    [SerializeField] private float waitAtTop = 1f;
    [SerializeField] private float waitAtBottom = 1f;

    private Vector3 startPosition;
    private Vector3 topPosition;

    private void Start()
    {
        startPosition = transform.position;
        topPosition = startPosition + Vector3.up * height;

        StartCoroutine(ElevatorLoop());
    }

    private IEnumerator ElevatorLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitAtBottom);

            // Subir
            while (Vector3.Distance(transform.position, topPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    topPosition,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            yield return new WaitForSeconds(waitAtTop);

            // Bajar
            while (Vector3.Distance(transform.position, startPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    startPosition,
                    speed * Time.deltaTime
                );

                yield return null;
            }
        }
    }
}