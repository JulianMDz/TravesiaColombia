using System.Collections;
using UnityEngine;

public class FallingBananaLoop : MonoBehaviour
{
    [Header("Altura desde donde aparece")]
    [SerializeField] private float spawnHeight = 10f;

    [Header("Movimiento")]
    [SerializeField] private float fallSpeed = 6f;

    [Header("Tiempos")]
    [SerializeField] private float waitBeforeFirstFall = 0.5f;
    [SerializeField] private float waitOnGround = 2f;
    [SerializeField] private float waitBeforeNextFall = 1f;

    [Header("Visual")]
    [SerializeField] private bool hideWhenResetting = true;

    private Vector3 groundPosition;
    private Vector3 startPosition;

    private Renderer[] renderers;
    private Collider[] colliders;

    private void Start()
    {
        // La posición donde pusiste la banana en Unity será el punto donde cae.
        groundPosition = transform.position;

        // Calcula la posición de aparición arriba.
        startPosition = groundPosition + Vector3.up * spawnHeight;

        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();

        transform.position = startPosition;

        StartCoroutine(FallLoop());
    }

    private IEnumerator FallLoop()
    {
        yield return new WaitForSeconds(waitBeforeFirstFall);

        while (true)
        {
            // Aparece arriba
            transform.position = startPosition;
            SetVisible(true);
            SetColliders(true);

            // Cae hasta donde estaba ubicada originalmente
            while (Vector3.Distance(transform.position, groundPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    groundPosition,
                    fallSpeed * Time.deltaTime
                );

                yield return null;
            }

            transform.position = groundPosition;

            // Se queda en el suelo un momento
            yield return new WaitForSeconds(waitOnGround);

            // Desaparece antes de volver arriba
            if (hideWhenResetting)
            {
                SetVisible(false);
                SetColliders(false);
            }

            // Vuelve arriba sin animación
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