using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    public float rotationFactor;

    public float movementFactor;

    //Inverse mass for efficiency
    public float invMass;

    //Inverse boost for efficiency
    public float invBoost;

    //Represents the amount of boost "nitro"
    public int currentBoost;

    public float maxBoostVel = 2.5f;

    float boostFactor = 1;

    private Rigidbody2D rb;

    private Ship playerShip;

    public Slider boostSlider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerShip = GetComponent<Ship>();
        rb.angularDrag = playerShip.rotationTorque * 3;
    }

    public void UpdateStats()//Called whenever the ship gets updated, such as when a node gets destroyed
    {
        invBoost = 1 / (float)playerShip.maxBoost;
        invMass = 1 / rb.mass;
    }

	// Update is called once per frame
	void FixedUpdate () {

        //The inputs for the player
        float horizonal = Input.GetAxis("Horizontal");
        float vertical = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);

        float offsetedDirection = rb.rotation + GameManager.instance.player.direction;

        //Gets a vector 2 from rotation
        Vector2 rotation = new Vector2(Mathf.Cos(offsetedDirection * Mathf.Deg2Rad), Mathf.Sin(offsetedDirection * Mathf.Deg2Rad));

        if (vertical > 0)
        rb.AddForce((rotation * playerShip.moveSpeed * vertical * movementFactor * boostFactor), ForceMode2D.Impulse);

        if(horizonal != 0)
        rb.angularVelocity = (-horizonal * playerShip.rotationTorque * rotationFactor) * invMass;

        //Code below handles the boost mechanism - it's a little complicated because of the interplay between how player movement works, and that I wanted a smooth boost

        bool usedBoost = false;

        //Boost input
        if (Input.GetButton("Secondary"))
        {
            //Increase the boost smoothly
            if (currentBoost > 0)
            {
                boostFactor = Mathf.Clamp(boostFactor + 0.1f, 1, maxBoostVel);
            }
            usedBoost = true;
        }

        //Reduce the boost smoothly
        if (currentBoost <= 0 || !usedBoost)
        {
            boostFactor = Mathf.Clamp(boostFactor - 0.1f, 1, maxBoostVel);
        }

        //Recharge the boost meter
        if (currentBoost < playerShip.maxBoost && !usedBoost)
        {
            currentBoost++;
        }
        
        //Deplete the boost meter if in use
        if (usedBoost && currentBoost > 0)
        {
            currentBoost -= 4;
        }

        //Keep boost meter from overfilling?!
        if(currentBoost > playerShip.maxBoost)
        {
            currentBoost = playerShip.maxBoost;
        }

        //The visual boostmeter
        boostSlider.value = currentBoost * invBoost;

    }

}
