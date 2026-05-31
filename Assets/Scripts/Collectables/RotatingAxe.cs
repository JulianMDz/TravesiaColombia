using UnityEngine;

public class RotatingAxe : MonoBehaviour
{
    [SerializeField] private float minSpeed = 80f;
    [SerializeField] private float maxSpeed = 180f;
    [SerializeField] private float variation = 25f;

    private float _baseSpeed;
    private float _offset;

    private void Start()
    {
        _baseSpeed = Random.Range(minSpeed, maxSpeed);

        if (Random.value > 0.5f)
            _baseSpeed *= -1f;

        _offset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float currentSpeed =
            _baseSpeed +
            Mathf.Sin(Time.time + _offset) * variation;

        transform.Rotate(
            0f,
            0f,
            currentSpeed * Time.deltaTime
        );
    }
}