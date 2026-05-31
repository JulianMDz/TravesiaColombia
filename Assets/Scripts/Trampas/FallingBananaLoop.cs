using System.Collections;
using UnityEngine;

public class FallingBanana : MonoBehaviour
{
    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private float fallSpeed = 20f;

    private Vector3 groundPosition;
    private Vector3 startPosition;

    private Renderer[] renderers;
    private Collider[] colliders;

    private bool falling = false;

    private void Start()
    {
        groundPosition = transform.position;
        startPosition = groundPosition + Vector3.up * spawnHeight;

        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();

        transform.position = startPosition;

        SetVisible(false);
        SetColliders(false);
    }

    public void DropBanana()
    {
        if (falling) return;

        StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        falling = true;

        transform.position = startPosition;

        SetVisible(true);
        SetColliders(true);

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

        // desaparece apenas cae
        SetVisible(false);
        SetColliders(false);

        falling = false;
    }

    private void SetVisible(bool state)
    {
        foreach (Renderer r in renderers)
            if (r != null)
                r.enabled = state;
    }

    private void SetColliders(bool state)
    {
        foreach (Collider c in colliders)
            if (c != null)
                c.enabled = state;
    }
}