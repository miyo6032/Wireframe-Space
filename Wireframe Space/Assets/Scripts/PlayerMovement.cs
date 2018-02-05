using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    public float rotationFactor;

    public float movementFactor;

    public float invMass;

    public float invBoost;

    public int currentBoost;

    float boostFactor = 1;

    private Rigidbody2D rb;

    private Ship playerShip;

    public Slider boostSlider;

    // Use this for initialization
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

        float horizonal = Input.GetAxis("Horizontal");//Movement for the player
        float vertical = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);

        float offsetedDirection = rb.rotation + GameManager.instance.player.direction;

        Vector2 rotation = new Vector2(Mathf.Cos(offsetedDirection * Mathf.Deg2Rad), Mathf.Sin(offsetedDirection * Mathf.Deg2Rad));//Gets a vector 2 from rotation

        if (vertical > 0)
        rb.velocity = (rotation * playerShip.moveSpeed * vertical * movementFactor * boostFactor) * invMass;

        if(horizonal != 0)
        rb.angularVelocity = (-horizonal * playerShip.rotationTorque * rotationFactor) * invMass;

        //Code below handles the boost mechanism - it's a little complicated because of the interplay between how player movement works, and that I wanted a smooth boost

        bool usedBoost = false;

        if (Input.GetButton("Secondary"))
        {
            if (currentBoost > 0)
            {
                boostFactor = Mathf.Clamp(boostFactor + 0.1f, 1, 2.5f);
            }
            usedBoost = true;
        }

        if (currentBoost <= 0 || !usedBoost)
        {
            boostFactor = Mathf.Clamp(boostFactor - 0.1f, 1, 2.5f);
        }

        if (currentBoost < playerShip.maxBoost && !usedBoost)
        {
            currentBoost++;
        }
        
        if (usedBoost && currentBoost > 0)
        {
            currentBoost -= 4;
        }

        if(currentBoost > playerShip.maxBoost)
        {
            currentBoost = playerShip.maxBoost;
        }

        boostSlider.value = currentBoost * invBoost;

    }

}
