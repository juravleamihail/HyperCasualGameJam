using UnityEngine;

public class CirclePoint : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        int a = 3;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int a = 3;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int a = 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        int a = 3;
    }
}
