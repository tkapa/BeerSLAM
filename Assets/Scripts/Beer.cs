﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beer : MonoBehaviour {

    //Know how hard the player wants to throw and which player wants to throw it
    public Vector3 throwingVector;

    public Rigidbody2D rb;

    bool isActive = false;

    public float defaultRebound = 500.0f;
    public float rollForce = 75.0f;
    //Time this object is invulnerable
    float invulTime = 0.3f;
    float invulCount;

	// Use this for initialization
	void Start () {
        EventManager.instance.OnEndRound.AddListener((b)=> { Destroy(this.gameObject); });

        defaultRebound = Random.Range(200.0f, 800.0f);

        //If there's no Rigidbody
        if (!GetComponent<Rigidbody2D>())
            Debug.LogError("Beer prefab does not contain a RigidBody!");
        else
            rb = GetComponent<Rigidbody2D>();

        invulCount = Time.time + invulTime;
    }

    // Update is called once per frame
    void Update () {
                
        if(!isActive && Time.time > invulCount)
        {
            isActive = true;
            GetComponent<CircleCollider2D>().isTrigger = false;
        }
	}

    //Called when a beer can hits another beer can
    void PushBack(GameObject o)
    {
        float totalForce = defaultRebound;

        totalForce += o.GetComponent<Rigidbody2D>().velocity.magnitude + rb.velocity.magnitude;

        Vector2 direction = o.transform.position - transform.position;
        rb.AddForce(-direction * totalForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "beer")
        {
            FindObjectOfType<EffectsManager>().BeerCollision(collision.contacts[0].point);
            PushBack(collision.gameObject);
        }

        if(collision.gameObject.tag == "ground")
        {
            float xVel = rb.velocity.x;
            rb.AddForce(new Vector2(Mathf.Clamp(xVel, -1, 1) * rollForce, 0));
        }
    }
}
