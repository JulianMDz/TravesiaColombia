using UnityEngine;

public class SwingingLog : MonoBehaviour
{
    [SerializeField] private float maxAngle = 40f;
    [SerializeField] private float speed = 2f;

    private float _offset;

    private void Start()
    {
        int index = transform.GetSiblingIndex();

        _offset =
            (index % 2 == 0)
            ? 0f
            : Mathf.PI;
    }

    private void Update()
    {
        float angle =
            Mathf.Sin(Time.time * speed + _offset)
            * maxAngle;

        transform.localRotation =
            Quaternion.Euler(
                angle,
                0f,
                0f
            );
    }
}