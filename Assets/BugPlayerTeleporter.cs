using UnityEngine;

public class BugPlayerTeleporter : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = Vector2.zero;
        }
    }
}
