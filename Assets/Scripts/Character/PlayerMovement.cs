using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed = 5.0f;
    [SerializeField] int _maxHealth = 3;
    

    private Animator _animController;
    private Animator _slashAnimator;
    private SpriteRenderer _spriteRenderer;
    private GameObject _slash;
    private bool canAttack = true;
    private int currentHealth;


    void Start()
    {
        currentHealth = _maxHealth;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animController = GetComponent<Animator>();
        foreach( Transform child in transform) {
            if(child.gameObject.tag == "SlashAttack") {
                _slash = child.gameObject;

                _slashAnimator = _slash.GetComponent<Animator>();
            }
        }
    }
    void Update()
    {
        Move();
        Attack();
    }

    private void Move() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if(horizontalInput != 0f || verticalInput != 0f) {
            _animController.SetBool("Walk", true);

            if(horizontalInput > 0f) {
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
            else {
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
            }

            Vector2 direction = new Vector2(horizontalInput, verticalInput);
            transform.Translate(direction * _speed * Time.deltaTime);

        }
        else {
            _animController.SetBool("Walk", false);
        }

    }

    private void Attack() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (canAttack) {
                _slash.SetActive(true);
                _slashAnimator.SetTrigger("Attack");
                canAttack = false;
            }
        }
    }

    public void TakeDamage(bool isLeft) {
        if (isLeft) {
            transform.Translate(new Vector2(0.3f, 0));
        }
        else {
            transform.Translate(new Vector2(-0.3f, 0));
        }

        _spriteRenderer.color = Color.red;
        StartCoroutine(ReturnColorWhite());
        currentHealth -= 1;

        if (currentHealth <= 0) {
            Die();
        }
    }

    private IEnumerator ReturnColorWhite() {
        yield return new WaitForSeconds(0.05f);

        _spriteRenderer.color = Color.white;
    }

    private void Die() { 
        //TODO: end game
    }
    public void FinishAttack() {
        _slash.SetActive(false);
        canAttack = true;
    }
}
