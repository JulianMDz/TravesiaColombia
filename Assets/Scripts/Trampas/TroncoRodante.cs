using UnityEngine;

public class TroncoRodante : MonoBehaviour
{
    private Rigidbody rb;
    private bool activado = false;

    [Header("Movimiento")]
    public Vector3 direccionRodar = Vector3.forward;
    public float fuerza = 20f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void Activar()
    {
        if (activado) return;

        activado = true;

        rb.isKinematic = false;

        // Impulso inicial
        rb.AddForce(direccionRodar.normalized * fuerza, ForceMode.Impulse);

        // Torque para que realmente ruede
        rb.AddTorque(Vector3.forward * fuerza, ForceMode.Impulse);
    }
}