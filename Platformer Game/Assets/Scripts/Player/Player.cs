using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour { 

    public float blendTime;

    private float moveSpeed;
    private float lastMoveSpeed;
    private float speed;

    [Header("References")]
    public PlayerRb pm;
    public Rigidbody rb;
    public Animator animator;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerRb>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update(){
        AnimationHandler();
    }

    /*
    void OnTriggerEnter(Collider collider)
    {
        ItemWorld itemWorld = collider.GetComponent<ItemWorld>();
        if(itemWorld != null)
        {
            GameManager.instance.inventory.AddItem(itemWorld.GetItem());
            itemWorld.DestroySelf();
        }
    }*/

    void AnimationHandler()
    {
        moveSpeed = rb.velocity.magnitude/10;
        if (pm.isGrounded)
        {
            //if(lastMoveSpeed != moveSpeed)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpAnimationSpeed());
                //DOTween.To(()=>speed, x=> speed = x, moveSpeed, blendSpeed);
                animator.SetFloat("Speed", speed);
            }
        } else animator.SetFloat("Speed", 0);

        //lastMoveSpeed = moveSpeed;
    }
    IEnumerator SmoothlyLerpAnimationSpeed(){
        float currentTime = 0;
        while (currentTime < blendTime){
            currentTime += Time.deltaTime;

            float step = Mathf.Clamp01(currentTime/blendTime);
            speed = Mathf.Lerp(speed, moveSpeed, step);

            yield return null;
        }
    }
}
