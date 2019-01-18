﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementPlayer : MonoBehaviour {

    #region variables

    //inputs
    [SerializeField]
    [Tooltip("Gameplay Inputs.")]
    private string HorizontalMovementInput, GravityChangeInput;

    //velocidade
    [SerializeField]
    [Tooltip("Movement speed of player.")]
    private float MovementSpeed;

    [SerializeField]
    [Tooltip("Limit in units that defines the edge of the world.")]
    private float HorizontalLimit;

    //estado
    private bool falling = false;

    //gravidade
    [SerializeField]
    [Tooltip("Interval during which player cannot change gravity.")]
    private float GravityChangeInterval;
    private bool CanChangeGravity = true;
    [HideInInspector]
    public float TimeToChangeGravity = 0f;

    //animation
    private Animator animator;

    #endregion

    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    void Update () {

        RestrictRotation();
        ManageInputs();
		
	}

    //eventos em colisões
    private void OnCollisionEnter2D(Collision2D collision)
    {

        print("collision found");

        //se encontrar floors
        if (collision.gameObject.CompareTag("Floor")) {

            animator.SetTrigger("hitFloor");

            falling = false;

        }

        falling = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        animator.ResetTrigger("walk");
        animator.ResetTrigger("stopMoving");
        animator.ResetTrigger("hitFloor");

        animator.SetTrigger("fall");

        falling = true;
    }

    //restringir rotação
    private void RestrictRotation()
    {
        this.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    //lidar com inputs
    private void ManageInputs()
    {
        float horizontal = Input.GetAxisRaw(HorizontalMovementInput);
        float switchGravity = Input.GetAxisRaw(GravityChangeInput);


        if(horizontal != 0 && !falling)
        {
            animator.ResetTrigger("fall");
            animator.ResetTrigger("hitFloor");
            animator.ResetTrigger("stopMoving");

            animator.SetTrigger("walk");
        }

        else if (!falling)
        {
            animator.ResetTrigger("fall");
            animator.ResetTrigger("walk");

            animator.SetTrigger("stopMoving");
        }

        #region moving
        if (horizontal != 0)
        {
            Vector3 newPosition = transform.position;
            newPosition += new Vector3( horizontal * MovementSpeed * Time.deltaTime, 0, 0);
            if (newPosition.x > transform.position.x && transform.localScale.x < 0)
            {
                flipObjectHorizontal();
            }

            else if (newPosition.x < transform.position.x && transform.localScale.x > 0)
            {
                flipObjectHorizontal();
            }



            newPosition.x = Mathf.Clamp(newPosition.x, -HorizontalLimit, HorizontalLimit);
            transform.position = newPosition;
        }
        #endregion

        #region gravity changing
        if (switchGravity != 0 && TimeToChangeGravity < Time.time)
        {   
            TimeToChangeGravity = Time.time + GravityChangeInterval;
            CanChangeGravity = false;
            this.GetComponent<Rigidbody2D>().gravityScale = -this.GetComponent<Rigidbody2D>().gravityScale*4;
            flipObjectVertical();

            falling = true;

            //animations
            animator.ResetTrigger("hitFloor");
            animator.SetTrigger("fall");
        }

        if (this.GetComponent<Rigidbody2D>().gravityScale > 1)
        {
            this.GetComponent<Rigidbody2D>().gravityScale = Mathf.Lerp(this.GetComponent<Rigidbody2D>().gravityScale, 1, 0.15f);
        }

        else if (this.GetComponent<Rigidbody2D>().gravityScale < -1)
        {
            this.GetComponent<Rigidbody2D>().gravityScale = Mathf.Lerp(this.GetComponent<Rigidbody2D>().gravityScale, -1, 0.15f);
        }
        #endregion
    }

    #region flip object control

    //inverter vertical
    private void flipObjectVertical()
    {
        print("inverted");
        Vector3 OriginalLocalScale = this.transform.localScale;
        OriginalLocalScale.y *= -1;
        this.transform.localScale = OriginalLocalScale;
        
    }

    //inverter horizontal
    private void flipObjectHorizontal()
    {
        print("inverted");
        Vector3 OriginalLocalScale = this.transform.localScale;
        OriginalLocalScale.x *= -1;
        this.transform.localScale = OriginalLocalScale;

    }
    #endregion
}


