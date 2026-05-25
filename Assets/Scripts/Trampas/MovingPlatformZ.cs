using System.Collections;
using UnityEngine;

public class MovingPlatformZ : MonoBehaviour
{
    [Header("Movimiento en eje Z")]
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float speed = 2f;

    [Header("Tiempos de espera")]
    [SerializeField] private float waitAtEnd = 1f;
    [SerializeField] private float waitAtStart = 1f;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + Vector3.forward * moveDistance;

        StartCoroutine(MoveLoop());
    }

    private IEnumerator MoveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitAtStart);

            // Mover hacia adelante en Z
            while (Vector3.Distance(transform.position, endPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    endPosition,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            yield return new WaitForSeconds(waitAtEnd);

            // Volver a la posición original
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