using UnityEngine;

public class LawnMover : MonoBehaviour
{
    Rigidbody2D rb;
    bool moving;
    private readonly float moveSpeed = 150f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Zombie"))
            return;

        collision.GetComponent<BaseZombie>().TakeDamage(9999999, transform.position);
        if (!moving)
        {
            AudioManager.Instance.sfxPool.PlaySFX("lawnmower");
            moving = true;
            rb.velocity = moveSpeed * Time.deltaTime * Vector2.right;
        }
    }
}
