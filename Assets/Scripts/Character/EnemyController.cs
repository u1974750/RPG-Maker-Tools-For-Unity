using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    private enum State { Walk, Idle, Attack, Follow };

    public bool isMelee;
    public bool isPattrol;
    public int maxHealth = 3;
    public int damage = 3;
    public GameObject[] pattrolPoints;

    private int currentHealth;
    private int currentPattrol = 0;
    private int maxPattrol;
    private int speed = 3;
    private bool isFacingRight = true;
    private State actualState;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private GameObject player;
    private GameObject slash;
    private GameObject bullet;
    private Animator slashAnimator;
    private bool isIdle = false;
    private bool playerExitRange = false;
    private bool canAttack = true;
    private float maxFollowCounter = 5f;
    private float followCounter = 5f;
    private float distanceToAttack = 1.05f;


    // Start is called before the first frame update
    void Start() {
        if (!isMelee) {
            bullet = Resources.Load<GameObject>("Bullet");
            distanceToAttack = 3.5f;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        maxPattrol = pattrolPoints.Length;

        if (isPattrol) actualState = State.Walk;
        else actualState = State.Idle;

        foreach (Transform child in transform) {
            if (child.gameObject.tag == "SlashAttack") {
                slash = child.gameObject;

                slashAnimator = slash.GetComponent<Animator>();
            }
        }
    }

    // Update is called once per frame
    void Update() {

        if (isPattrol) {
            if (pattrolPoints.Length > 0) {
                if (actualState == State.Walk) {
                    animator.SetBool("Walk", true);

                    transform.position = Vector2.MoveTowards(transform.position, pattrolPoints[currentPattrol].transform.position, speed * Time.deltaTime);
                }
                else if (actualState == State.Follow) {
                    animator.SetBool("Walk", true);
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

                    if (playerExitRange) {
                        followCounter -= 1 * Time.deltaTime;

                        if (followCounter < 0) {
                            actualState = State.Walk;
                        }
                    }


                }
            }
        }

        //calcula distancia per atacar
        if (player != null) {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= distanceToAttack && !playerExitRange) {
                actualState = State.Attack;
            }
        }


        if (actualState == State.Attack) {
            Attack();
        }
    }

    private void Attack() {
        if (isMelee) MeleeAttack();
        else DistanceAttack();
    }

    private void MeleeAttack() {
        if (canAttack) {
            if (player.transform.position.x > transform.position.x && !isFacingRight) {
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                isFacingRight = true;

            }
            else if (player.transform.position.x < transform.position.x && isFacingRight) {
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
                isFacingRight = false;
            }
            slash.SetActive(true);
            slashAnimator.SetTrigger("Attack");
            canAttack = false;
        }
    }

    private void DistanceAttack() {
        if (player != null && canAttack) {
            Vector2 fireDirection = new Vector2(player.transform.position.x, player.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
            GameObject newBullet = Instantiate(bullet, transform);
            newBullet.GetComponent<Bullet>().SetDirection(fireDirection);

            canAttack = false;
            StartCoroutine(AttackTimeWait());
        }
    }

    public void FinishAttack() {
        slash.SetActive(false);
        StartCoroutine(AttackTimeWait());
    }

    private void Idle(bool fromAttack) {
        isIdle = false;
        animator.SetBool("Walk", false);
        if (!fromAttack) {
            currentPattrol++;
            if (currentPattrol >= maxPattrol) currentPattrol = 0;
        }
        StartCoroutine(IdleTimmingWait());
    }

    public void TakeDamage(bool isLeft, int damage) {
        if (isLeft) {
            transform.Translate(new Vector2(0.3f, 0));
        }
        else {
            transform.Translate(new Vector2(-0.3f, 0));
        }

        spriteRenderer.color = Color.red;
        StartCoroutine(ReturnColorWhite());
        currentHealth -= damage;

        if (currentHealth <= 0) {
            Die();
        }
    }

    private void Die() {
        Destroy(gameObject);
    }

    private IEnumerator ReturnColorWhite() {
        yield return new WaitForSeconds(0.05f);

        spriteRenderer.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "pattrolPoint" && actualState == State.Walk) {
            actualState = State.Idle;
            isIdle = true;
            Idle(false);
        }
        else if (other.gameObject.tag == "Player") {
            player = other.gameObject;
            if (isMelee) {
                playerExitRange = false;
                followCounter = maxFollowCounter;
                actualState = State.Follow;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            if (isMelee) {
                playerExitRange = true;
                actualState = State.Idle;
                Idle(true);
            }
            else {
                if (isPattrol) actualState = State.Walk;
                else {
                    actualState = State.Idle;
                }
            }
        }
    }

    private IEnumerator IdleTimmingWait() {
        yield return new WaitForSeconds(5f);
        if (pattrolPoints[currentPattrol].transform.position.x > gameObject.transform.position.x) {
            if (!isFacingRight) {
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                isFacingRight = true;
            }
        }
        else {
            if (isFacingRight) {
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
                isFacingRight = false;
            }
        }

        actualState = State.Walk;

    }

    private IEnumerator AttackTimeWait() {
        float secondsToWait = isMelee ? 0.5f : 1f;
        yield return new WaitForSeconds(secondsToWait);
        canAttack = true;


        //if (isPattrol) actualState = State.Walk;
        //else actualState = State.Idle;
    }
}
