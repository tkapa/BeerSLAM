﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour {

    //Used to define which number player this is
    public enum Player_Number{
        EPN_1 = 0,
        EPN_2
    }

    //Tracks the state of the player
    public enum Player_State
    {
        EPS_Standing,
        EPS_Jumping,
        EPS_Dodging,
        EPS_Parrying,
        EPS_Ducking
    }

    [Tooltip("Number for player")]
    public Player_Number playerNumber;

    [HideInInspector]
    public Player_State playerState = Player_State.EPS_Standing;

    [HideInInspector]
    public Rigidbody2D rb;

    public Transform arm;
    public Transform point;

    //Inputs for this player
    public string throwInput, dodgeInput;

    public float jumpingForce = 1500.0f;

    //Am I meant to be able to move
    //(is true for now, make false when testing)
    bool takingInput = false;

    //Manages the player's throwing and when the strength increases 
    public Vector2 throwThresholds = new Vector2(0.4f, 0.8f);
    private float throwHoldTime = 0.0f;

    public Vector2 lowThrowStrength, medThrowStrength, highThrowStrength;

    //Manages the player's count to jump or dodge
    public Vector3 dodgeThresholds = new Vector3(0.3f, 0.4f, 0.8f);
    private float jumpTimerStart = 0;
    public float dodgeTime;
    //Beer can gameObject
    public GameObject beerCan;
    Vector2 startPos;

    // Use this for initialization
    void Start () {
        startPos = this.transform.position;
        //Check for setup errors
        if (!GetComponent<Rigidbody2D>())
            Debug.LogError(gameObject.name + " does not contain a rigidbody2D!");
        else rb = GetComponent<Rigidbody2D>();

        //Listen for round events
        EventManager.instance.OnBeginRound.AddListener(() => {
            takingInput = true;
        });

        EventManager.instance.OnEndRound.AddListener((b) => {
            takingInput = false;
            ResetPosition();
        });
    }
	
	// Update is called once per frame
	void Update () {
        RotateArm(Time.deltaTime);

        //If input is being accepted
        if (takingInput)
            PollInput(Time.deltaTime);

    }

    //Called to take input from the player
    void PollInput(float timeDelta)
    {
        //
        //If the dodge button is pressed start the timer
        //if the button is pressed again within the time frame jump
        //else stop time, don't jump,reset counter

        //

        //While holding down the throw button
        if (Input.GetKey(throwInput))
        {
            throwHoldTime += timeDelta;

            //Ensure to provide feedback to the player
        }
        //When the player releases the throw button
        else if (Input.GetKeyUp(throwInput))
        {
            Throw();

            //Reset the hold time
            throwHoldTime = 0.0f;
        }

        //When the dodge button is presssed
        if (Input.GetKey(dodgeInput))
        {
            //Player jumps if they press the dodge button twice
            //Player dodges if they hold it down for a little bit
            //Player ducks for however long they want after those                

            dodgeTime += timeDelta;

        }
        else if (Input.GetKeyUp(dodgeInput))
        {
            dodgeTime = 0;

        }

        //
        CheckDodgeInput(timeDelta); //Dodge,jump,duck
    }

    //Called when the player is hit
    public void OnDeath()
    {
        if (playerNumber == Player_Number.EPN_1)
            EventManager.instance.OnEndRound.Invoke(1);
        else
            EventManager.instance.OnEndRound.Invoke(0);
    }

    void Throw()
    {
        if (throwHoldTime > throwThresholds.y)
        {

            switch (playerState)
            {
                //Throw for standing
                case Player_State.EPS_Standing:
                    GameObject b = Instantiate(beerCan, arm.position, arm.rotation) as GameObject;
                    b.GetComponent<Rigidbody2D>().AddForce(highThrowStrength);
                    Destroy(b, 4.0f);
                    break;

                //Throw for jumping
                case Player_State.EPS_Jumping:
                    GameObject g = Instantiate(beerCan, arm.position, arm.rotation) as GameObject;
                    g.GetComponent<Rigidbody2D>().AddForce(lowThrowStrength);
                    Destroy(g, 4.0f);
                    break;
            }
        }
        else if (throwHoldTime > throwThresholds.x && throwHoldTime < throwThresholds.y)
        {
            switch (playerState)
            {
                //Throw for standing
                case Player_State.EPS_Standing:
                    GameObject b = Instantiate(beerCan, arm.position, arm.rotation) as GameObject;
                    b.GetComponent<Rigidbody2D>().AddForce(medThrowStrength);
                    Destroy(b, 4.0f);
                    break;

                //Throw for jumping
                case Player_State.EPS_Jumping:
                    GameObject g = Instantiate(beerCan, arm.position, arm.rotation) as GameObject;
                    g.GetComponent<Rigidbody2D>().AddForce(lowThrowStrength);
                    Destroy(g, 4.0f);
                    break;
            }
        }
        else if (throwHoldTime < throwThresholds.x)
        {
            switch (playerState)
            {
                //Throw for standing
                case Player_State.EPS_Standing:
                    GameObject b = Instantiate(beerCan, arm.position, arm.rotation) as GameObject;
                    b.GetComponent<Rigidbody2D>().AddForce(lowThrowStrength);
                    Destroy(b, 4.0f);
                    break;

                //Throw for jumping
                case Player_State.EPS_Jumping:
                    GameObject g = Instantiate(beerCan, arm.position, arm.rotation) as GameObject;
                    g.GetComponent<Rigidbody2D>().AddForce(lowThrowStrength);
                    Destroy(g, 4.0f);
                    break;
            }
        }
    }

    void Duck()
    {
        print("duck");
    }

    //Is called when the player goes to ttry to Jump,duck or dodge
    void CheckDodgeInput(float d)
    {
        if (Input.GetKeyDown(dodgeInput))
        {
            if (dodgeTime < dodgeThresholds.x) {
                if (playerState == Player_State.EPS_Standing)
                {
                    if (Time.time < jumpTimerStart + dodgeThresholds.x)
                        Jump();
                        
                }
                jumpTimerStart = Time.time;
            }
            else if (dodgeTime > dodgeThresholds.x && dodgeTime <= dodgeThresholds.y)
            {
                //in here
                print("dodge");
            }
            //Duck
            else if (dodgeTime > dodgeThresholds.y)
            {
                //in here
                
                Duck();
            }
            
        }
        
    }

    //Called when the player wants to jump
    void Jump()
    {
        //Set to jumping and start to jump
        playerState = Player_State.EPS_Jumping;

        rb.AddForce(new Vector2(0, jumpingForce));
    }

    //Coroutine begins wheen the player dodges
    IEnumerator Dodge()
    {
        playerState = Player_State.EPS_Dodging;
        //Player jumps backwards
        //Player is set to dodging state while jumping backwards
        //Player jumps forwasrds again
        //Player is set to Parry state during this time
        //Player returns to origin position and is set to standing again
        yield return null;
    }

    //Reset the player's position to the origin
    void ResetPosition()
    {
        transform.position = startPos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Upon collision with the ground
        if (collision.gameObject.tag == "ground" && playerState == Player_State.EPS_Jumping)
            playerState = Player_State.EPS_Standing;

        //Upon collision with a beer can
        if (collision.gameObject.tag == "beer")
        {
            OnDeath();
            Destroy(collision.gameObject);
        }
            

        //Provide screenshake here
    }

    void RotateArm(float d)
    {
        float angle = 4.0f;
        float rSpeed = 5f;
        float radius;
    }
}
