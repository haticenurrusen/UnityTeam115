using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    bool isGrounded, isRunning, isFacingLeft;
    public float speed, jump;
    float inputX;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update() {
        rb.velocity = new Vector2(inputX * speed, rb.velocity.y);
        if (isGrounded) {
            isRunning = (rb.velocity != Vector2.zero);
            anim.SetBool("running", isRunning);
        }
        if (!isFacingLeft && rb.velocity.x < 0) {
            FlipSprite();
        }
        else if (isFacingLeft && rb.velocity.x > 0) {
            FlipSprite();
        }
    }

    void FlipSprite() {
        isFacingLeft = !isFacingLeft;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void Move(InputAction.CallbackContext context) {
        inputX = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context) {
        if (context.performed && isGrounded) {
            rb.velocity = new Vector2(rb.velocity.x, jump);
        }
    }

    void OnTriggerEnter2D()
    {
        isGrounded = true;
    }
    void OnTriggerExit2D()
    {
        isGrounded = false;
    }

}
