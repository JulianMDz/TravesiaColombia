using UnityEngine;

public class ActivadorTroncos : MonoBehaviour
{
    public TroncoRodante[] troncos;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activado) return;

        if (other.CompareTag("Player"))
        {
            activado = true;

            foreach (TroncoRodante tronco in troncos)
            {
                tronco.Activar();
            }
        }
    }
}