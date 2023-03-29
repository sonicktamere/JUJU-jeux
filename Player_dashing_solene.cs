using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Il arrive que le dash en diagonal propulse Sunny loin dans la map.
C'est parce que, (pour l'exemple), le vector horizontal et vertical sont de 1, du coup en diagonale ça les additionnes donc ca fait 2.
Pour ça, il faut mettre une "normalize" et rajouter ses deux vecteurs ((donc x;1 et y;1)) pour qu'en diagonale ca fasse 0,5.
*/
public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animController;
    Vector2 ref_velocity = Vector2.zero;
    float moveSpeedHorizontal = 400f;
    float horizontalValue;
    float verticalValue;

    //SerializeField sert à afficher les paramètres dans le player
    //Le dash limit sert à mettre une Limite de Dash, je peux le changer directement dans le "Inspector" du player.
    [SerializeField] int dashLimit = 1;
    [SerializeField] int currentDash;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] bool isJumping = false;
    [SerializeField] bool canJump = false;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool hasVerticalChanged;
    //C'est la force du dash en vertical et horizontal. Sunny se tourne automatiquement vers la droite, à modifier. 
    [SerializeField] private float horizontalDashingPower = 24f;
    [SerializeField] private float verticalDashingPower = 14f;
    [SerializeField] private float dashingTime = 0.001f;
    //Le chiffre inscrit correspond au temps avant que Sunny puisse re-sauter. 
    [SerializeField] private float dashingCooldown = 1f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();
        currentDash = dashLimit;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }

        //Gère l'orientation du sprite en horizontal
        horizontalValue = Input.GetAxis("Horizontal");
        animController.SetBool("Running", horizontalValue != 0);
        animController.SetFloat("speed", Mathf.Abs(horizontalValue));

    
        verticalValue = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            isJumping = true;
            animController.SetBool("Jumping", true);
        }

        animController.SetFloat("Speed", Mathf.Abs(horizontalValue));

        //Rémi a dit qu'il était pas fan du Coroutine, donc si vous arrivez à modifier c'est tant mieux
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && currentDash > 0)
        {
            StartCoroutine(Dash());
        }

        Debug.Log(rb.velocity);
    }
    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        if (isJumping)
        {
            isJumping = false;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            canJump = false;
        }
        Vector2 target_velocity = new Vector2(horizontalValue * moveSpeedHorizontal * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, target_velocity, ref ref_velocity, 0.05f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        currentDash = dashLimit;
        canJump = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animController.SetBool("Jumping", false);
    }
    private IEnumerator Dash()
    {
        /* Ces 3 manières d'écrires reviennent au même résultat
        currentDash = currentDash - 1;
        currentDash -= 1; */
        currentDash--;
        canDash = false;
        // facing left 
        if (sr.flipX == true)
        {
            //Ligne de code permettant de dash de gauche à droite (attention au négatif)
            rb.velocity += new Vector2(transform.localScale.x * -horizontalDashingPower, transform.localScale.y);
        }
        else
        {
            rb.velocity += new Vector2(transform.localScale.x * horizontalDashingPower, transform.localScale.y);
        }

        if (hasVerticalChanged)
        {
            
            if (sr.flipY == true)
            {
                rb.velocity += new Vector2(transform.localScale.x, transform.localScale.y * -verticalDashingPower);
            }
            else
            {
                rb.velocity += new Vector2(transform.localScale.x, transform.localScale.y * verticalDashingPower);
            }
        }

        //Je crois que "return" est utilisé pour relancer le code dès que Sunny a toucher le sol
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}