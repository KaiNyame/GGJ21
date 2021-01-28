using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerControllerWithMovementAndState : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    public float moveSpeed;
    public float moveAcceleration;
    public float accelerationCap;
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) {
            //is moving
            if (moveAcceleration < accelerationCap) {
                moveAcceleration += 1 * Time.deltaTime;
            }
        }
        else {
            //should not be moving
            moveAcceleration = 1;
        }
        transform.Translate(new Vector3(horizontalInput, 0, verticalInput) * (moveSpeed * moveAcceleration * Time.deltaTime));
    }
}
