using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Com : MonoBehaviour
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
    [Range(0, 1)][SerializeField] float smooth_time = 0.5f;
    [SerializeField] private TrailRenderer tr;
    private bool wallJumped = false;

    // Vitesse de déplacement du joueur
    public float moveSpeed = 5f;
    // Distance maximale de déplacement lors du dash
    public float dashDistance = 5f;
    // Temps de recharge du dash en secondes
    public float dashCooldown = 1f;
    // Direction du dash
    private Vector2 dashDirection;
    // Temps restant avant de pouvoir utiliser le dash à nouveau
    private float dashCooldownTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();
        //Debug.Log(Mathf.Lerp(current, target, 0));
    }

    // Update is called once per frame
    void Update()
    {

        // Si le dash est en cours de recharge, mettre à jour le temps restant
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Sinon, permettre au joueur d'utiliser le dash
        else
        {
            // Récupérer le vecteur de direction
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector2 direction = new Vector2(horizontal, vertical).normalized;

            // Si le joueur appuie sur la touche de dash et que la direction est valide
            if (Input.GetKeyDown(KeyCode.LeftShift) && direction != Vector2.zero)
            {
                // Normaliser la direction pour éviter les déplacements excessifs en diagonale
                if (direction.magnitude > 1f)
                {
                    direction.Normalize();
                }

                // Stocker la direction pour le dash
                dashDirection = direction;

                // Mettre en place le temps de recharge du dash
                dashCooldownTimer = dashCooldown;
                tr.emitting = true;
            }
        }


        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            tr.emitting = false;
        }


        horizontal_value = Input.GetAxis("Horizontal");

        if (horizontal_value > 0) sr.flipX = false;
        else if (horizontal_value < 0) sr.flipX = true;


        /*vertical_value = Input.GetAxis("Verticale");

                if (vertical_value > 0) sr.flipY = false;
                else if (vertical_value < 0) sr.flipY = true;*/


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
        // Si le joueur est en train de dasher, le déplacer
        if (dashDirection != Vector2.zero)
        {
            //transform.position += dashDirection * dashDistance; //ca marche aussi avec ca mais ca te tp dans le sol

            //rb.velocity = dashDirection * dashDistance / Time.fixedDeltaTime; //envoie trop loin

            Vector2 targetPosition = rb.position + dashDirection * dashDistance; //CA MARCHE FINALEMENT

            rb.MovePosition(targetPosition);

            // Réinitialiser la direction du dash
            dashDirection = Vector2.zero;
        }
        // Sinon, déplacer le joueur normalement
        else
        {
            Vector2 target_velocity = new Vector2(horizontal_value * moveSpeed_horizontal * Time.fixedDeltaTime, rb.velocity.y);
            rb.velocity = Vector2.SmoothDamp(rb.velocity, target_velocity, ref ref_velocity, 0.05f);

        }


        if (is_jumping && can_jump)
        {
            is_jumping = false;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            can_jump = false;
        }

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


    //dashingTime -= Time.deltaTime;




}
       




