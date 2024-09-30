using UnityEngine;

public class Barrel : MonoBehaviour
{
    private new Rigidbody2D rigidbody;

    public float speed = 1f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rigidbody.AddForce(collision.transform.right * speed, ForceMode2D.Impulse);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Hammer hammer = collision.gameObject.GetComponent<Hammer>();
            if (hammer != null && hammer.IsHoldingHammer)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyOtherBarrels();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hammer"))
        {
            Destroy(gameObject);
        }
    }

    private void DestroyOtherBarrels()
    {
        GameObject[] barrels = GameObject.FindGameObjectsWithTag("Barrel");

        foreach (GameObject barrel in barrels)
        {
            if (barrel != gameObject)
            {
                Destroy(barrel);
            }
        }
    }
}
