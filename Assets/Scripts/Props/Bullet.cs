using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{

    [SerializeField] Rigidbody2D _rb;
    private float _speed = 2.0f;
    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDirection(Vector2 direction) {
        _rb.velocity = direction * _speed;
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            bool isLeft = false;
            if(transform.position.x < collision.transform.position.x) isLeft = true;
            collision.GetComponent<PlayerMovement>().TakeDamage(isLeft);

            Destroy(gameObject);

        }
    }

    private void OnBecameInvisible() {
        Destroy(gameObject);
    }
}
