using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animController;
    float horizontal_value;
    float vertical_value;
    Vector2 ref_velocity = Vector2.zero;
    float jumpForce = 12f;

    [SerializeField] float moveSpeed_horizontal = 400.0f;
    [SerializeField] bool is_jumping = false;
    [SerializeField] bool can_jump = false;
    [Range(0, 1)] [SerializeField] float smooth_time = 0.5f;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private bool hasVerticalChanged;
    [SerializeField] private bool hasHorizontalChanged;
    [SerializeField] private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.5f;
    private Vector2 DashDirection;
    [SerializeField] int DashPower = 0;
    
    
    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            canDash = false;
            Dash();
            tr.emitting = true;
        }



        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            tr.emitting = false;
        }

        if (isDashing)
        {
            print("test");
            rb.velocity =(DashDirection*DashPower);
 

            rb.velocity.Normalize();
            isDashing = false;
            canDash = true;
            Debug.Log("Test");

        }
        
        horizontal_value = Input.GetAxis("Horizontal");

        if (horizontal_value > 0) sr.flipX = false;
        else if (horizontal_value < 0) sr.flipX = true;


        animController.SetFloat("Speed", Mathf.Abs(horizontal_value));

        if (Input.GetButtonDown("Jump") && can_jump)
        {
            is_jumping = true;
            animController.SetBool("Jumping", true);
        }

        vertical_value = Input.GetAxis("Vertical");
    }
    void FixedUpdate()
    {
        if (is_jumping && can_jump)
        {
            is_jumping = false;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            can_jump = false;
        }
        Vector2 target_velocity = new Vector2(horizontal_value * moveSpeed_horizontal * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, target_velocity, ref ref_velocity, 0.05f);

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        can_jump = true;
        animController.SetBool("Jumping", false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        animController.SetBool("Jumping", false);
    }


    private void Dash()
    {
        if (dashingTime > 0)
        {
            isDashing = true;
            DashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            DashDirection.Normalize();
            Debug.Log(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));

            tr.emitting = true;
        }

    }
  
    




}
       






