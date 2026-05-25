using System.Collections;
using UnityEngine;

public class FallingLogLoop : MonoBehaviour
{
    [Header("Movimiento del tronco")]
    [SerializeField] private float fallDistance = 10f;
    [SerializeField] private float fallSpeed = 8f;

    [Header("Tiempos")]
    [SerializeField] private float waitBeforeFirstFall = 1f;
    [SerializeField] private float waitAtBottom = 0.2f;
    [SerializeField] private float waitBeforeNextFall = 1f;

    [Header("Efecto al reaparecer")]
    [SerializeField] private bool hideWhenResetting = true;

    private Vector3 startPosition;
    private Vector3 bottomPosition;
    private Renderer[] renderers;
    private Collider[] colliders;

    private void Start()
    {
        startPosition = transform.position;
        bottomPosition = startPosition + Vector3.down * fallDistance;

        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();

        StartCoroutine(FallLoop());
    }

    private IEnumerator FallLoop()
    {
        yield return new WaitForSeconds(waitBeforeFirstFall);

        while (true)
        {
            // Asegura que el tronco empiece arriba
            transform.position = startPosition;
            SetVisible(true);
            SetColliders(true);

            // Caída
            while (Vector3.Distance(transform.position, bottomPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    bottomPosition,
                    fallSpeed * Time.deltaTime
                );

                yield return null;
            }

            yield return new WaitForSeconds(waitAtBottom);

            // Se oculta antes de volver arriba
            if (hideWhenResetting)
            {
                SetVisible(false);
                SetColliders(false);
            }

            // Reaparece arriba instantáneamente
            transform.position = startPosition;

            yield return new WaitForSeconds(waitBeforeNextFall);
        }
    }

    private void SetVisible(bool state)
    {
        foreach (Renderer r in renderers)
        {
            if (r != null)
            {
                r.enabled = state;
            }
        }
    }

    private void SetColliders(bool state)
    {
        foreach (Collider c in colliders)
        {
            if (c != null)
            {
                c.enabled = state;
            }
        }
    }
}