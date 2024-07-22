using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{

    private Transform parentTransform;
    private Animator _anim;
    private bool animationPlayed = false;

    private bool isPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        AnimationEvent evt = new AnimationEvent();
        evt.time = 0.03f;
        evt.functionName = "InvokeFinishAttack";
        AnimationClip animationClip = gameObject.GetComponent<Animator>().runtimeAnimatorController.animationClips[0];
        //animationClip.AddEvent(evt);

        parentTransform = gameObject.transform.parent; 
        if(parentTransform.tag == "Player") isPlayer = true;
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("SlashAnimation")){
            animationPlayed = true;
        }
        else {
            if (animationPlayed) {
                InvokeFinishAttack();
                animationPlayed = false;
            }
        }
    }

    public void InvokeFinishAttack() {
        if (parentTransform != null) {
            if(parentTransform.gameObject.tag == "Player") {
                parentTransform.gameObject.GetComponent<PlayerMovement>().FinishAttack();
            }
            else {
                parentTransform.gameObject.GetComponent<EnemyController>().FinishAttack();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        bool isLeft = false;
        if(gameObject.transform.position.x < collision.transform.position.x) isLeft = true;
        if (isPlayer) {
            if(collision.gameObject.tag == "Enemy") {
                collision.GetComponent<EnemyController>().TakeDamage(isLeft, parentTransform.gameObject.GetComponent<PlayerMovement>().currentStats.strenghtValue);
            }
        }
        else {
            if( collision.gameObject.tag == "Player") {
                collision.GetComponent<PlayerMovement>().TakeDamage(isLeft, parentTransform.gameObject.GetComponent<EnemyController>().damage);
            }
        }
    }
}
