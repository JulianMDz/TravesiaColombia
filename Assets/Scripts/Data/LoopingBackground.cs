using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    [SerializeField] private float speed = 0.3f;
    [SerializeField] private float width = 30f;

    private void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -width)
        {
            transform.position += Vector3.right * width * 2f;
        }
    }
}