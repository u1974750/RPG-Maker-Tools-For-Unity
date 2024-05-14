using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private enum State { Walk, Idle };

    public bool isRange;
    public bool isPattrol;
    public GameObject[] pattrolPoints;

    private int currentPattrol = 0;
    private int maxPattrol;
    private int speed = 3;
    private State actualState;
    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        maxPattrol = pattrolPoints.Length;
        Debug.Log(maxPattrol);

        if(isPattrol)actualState = State.Walk;
        else actualState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {

        if (isPattrol) {
            if(pattrolPoints.Length > 0) {
                if(actualState == State.Walk) {
                    animator.SetBool("Walk", true);
                    
                    transform.position = Vector2.MoveTowards(transform.position, pattrolPoints[currentPattrol].transform.position, speed * Time.deltaTime);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "pattrolPoint") {
            actualState = State.Idle;
            animator.SetBool("Walk", false);
            currentPattrol++;
            if (currentPattrol >= maxPattrol) currentPattrol = 0;
            StartCoroutine(idleTimmingWait());
        }
    }

    private IEnumerator idleTimmingWait() {
        yield return new WaitForSeconds(5f);
        actualState = State.Walk;

    }
}
