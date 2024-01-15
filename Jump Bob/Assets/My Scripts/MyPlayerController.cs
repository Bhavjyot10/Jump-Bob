using SupanthaPaul;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SupanthaPaul
{
    public class MyPlayerController : MonoBehaviour
    {
        [SerializeField] private float speed;
        [Header("Jumping")]
        [SerializeField] private float jumpForce;
        [SerializeField] private float fallMultiplier;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private int extraJumpCount = 1;
        [SerializeField] private GameObject jumpEffect;
        [Header("Dashing")]
        [SerializeField] private float dashSpeed = 30f;
        [Tooltip("Amount of time (in seconds) the player will be in the dashing speed")]
        [SerializeField] private float startDashTime = 0.1f;
        [Tooltip("Time (in seconds) between dashes")]
        [SerializeField] private float dashCooldown = 0.2f;
        [SerializeField] private GameObject dashEffect;

        // Access needed for handling animation in Player script and other uses
        [HideInInspector] public bool isGrounded;
        [HideInInspector] public float moveInput;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool isDashing = false;
        // controls whether this instance is currently playable or not
        [HideInInspector] public bool isCurrentlyPlayable = false;

        private Rigidbody2D m_rb;
        private ParticleSystem m_dustParticle;
        private bool m_facingRight = true;
        private readonly float m_groundedRememberTime = 0.25f;
        private float m_groundedRemember = 0f;
        private int m_extraJumps;
        private float m_extraJumpForce;
        private float m_dashTime;
        private bool m_hasDashedInAir = false;
        private float m_dashCooldown;

        // 0 -> none, 1 -> right, -1 -> left
        private float m_playerSide = 1f;

        private JumpController jumpController;
        private bool canJumpDash = true;
        private bool canDash = false;
        // 0 -> none, 1 -> right, -1 -> left
        private int m_touchInput = 0;
        private bool m_touchJump = false;
        private bool m_DashBtnInput = false;

        public Text t;

        public UIManager uIManager;
        private bool stopJumpPhysics = false;

        private AudioManager audioManager;

        void Start()
        {
            // create pools for particles
            PoolManager.instance.CreatePool(dashEffect, 2);
            PoolManager.instance.CreatePool(jumpEffect, 2);

            // if it's the player, make this instance currently playable
            if (transform.CompareTag("Player"))
                isCurrentlyPlayable = true;

            m_extraJumps = extraJumpCount;
            m_dashTime = startDashTime;
            m_dashCooldown = dashCooldown;
            //m_extraJumpForce = jumpForce * 0.7f;
            m_extraJumpForce = jumpForce;

            m_rb = GetComponent<Rigidbody2D>();
            m_dustParticle = GetComponentInChildren<ParticleSystem>();
            jumpController = GetComponent<JumpController>();
            audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        }

        private void FixedUpdate()
        {
            // check if grounded
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            var position = transform.position;

            

            // if this instance is currently playable
            if (isCurrentlyPlayable)
            {
             /*   // horizontal movement
                if (canMove)
                    m_rb.velocity = new Vector2(moveInput * speed, m_rb.velocity.y);
                
                else if (!canMove)
                    m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
             */
                // better jump physics
                if (m_rb.velocity.y < 0f && !stopJumpPhysics)
                {
                    m_rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
                }

                

                // Dashing logic
                if (isDashing)
                {
                    if (m_dashTime <= 0f)
                    {
                        isDashing = false;
                        m_dashCooldown = dashCooldown;
                        m_dashTime = startDashTime;
                        m_rb.velocity = Vector2.zero;
                    }
                    else
                    {
                        m_dashTime -= Time.deltaTime;
                        if (m_facingRight)
                            m_rb.velocity = Vector2.right * dashSpeed;
                        else
                            m_rb.velocity = Vector2.left * dashSpeed;
                    }
                }

                // enable/disable dust particles
                float playerVelocityMag = m_rb.velocity.sqrMagnitude;
                if (m_dustParticle.isPlaying && playerVelocityMag == 0f)
                {
                    m_dustParticle.Stop();
                }
                else if (!m_dustParticle.isPlaying && playerVelocityMag > 0f)
                {
                    m_dustParticle.Play();
                }

            }
        }

        private void Update()
        {

            transform.position = new Vector3(Mathf.Max(-10, transform.position.x), Mathf.Clamp(transform.position.y, -27f, transform.position.y), 0);
            // horizontal input
            moveInput = InputSystem.HorizontalRaw();

            if (isGrounded)
            {
                m_extraJumps = extraJumpCount;
                m_rb.gravityScale = 4f;
                stopJumpPhysics = false;
            }

            // grounded remember offset (for more responsive jump)
            m_groundedRemember -= Time.deltaTime;
            if (isGrounded)
                m_groundedRemember = m_groundedRememberTime;

            if (!isCurrentlyPlayable) return;
            // if not currently dashing and hasn't already dashed in air once
            if (!isDashing && !m_hasDashedInAir && m_dashCooldown <= 0f && canJumpDash)
            {
                // dash input (left shift)
                if ((InputSystem.Dash() || m_DashBtnInput) && canDash)
                {
                    isDashing = true;
                    // dash effect
                    PoolManager.instance.ReuseObject(dashEffect, transform.position, Quaternion.identity);
                    // if player in air while dashing
                    if (!isGrounded)
                    {
                        m_hasDashedInAir = true;
                    }

                    jumpController.DashExhaustion(25);
                    DashStatus(false);

                    audioManager.PlayDashAudio();
                    // dash logic is in FixedUpdate
                }
            }
            m_DashBtnInput = false;
            m_dashCooldown -= Time.deltaTime;

            // if has dashed in air once but now grounded
            if (m_hasDashedInAir && isGrounded)
                m_hasDashedInAir = false;

            if(Input.touchCount > 0 && !uIManager.IsPointerOverUIObject())
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
                    pos.z = transform.position.z;

                    if (pos.x < transform.position.x)
                    {
                        m_touchInput = 1;
                        m_playerSide = 1;
                    }                    

                    else
                    {                        
                        m_touchInput = -1;
                        m_playerSide = -1;
                    }

                    m_touchJump = true;
                    
                }
            }

            CalculateSides();
            // Jumping
            if ((InputSystem.Jump() || m_touchJump) && m_extraJumps > 0 && !isGrounded && canJumpDash)   // extra jumping
            {
                m_rb.velocity = new Vector2(m_playerSide * speed, m_extraJumpForce);
                m_extraJumps--;
                // jumpEffect
                PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
                jumpController.JumpExhaustion(20);
                audioManager.PlayJumpAudio();
            }
            else if ((InputSystem.Jump() || m_touchJump) && (isGrounded || m_groundedRemember > 0f) && canJumpDash) // normal single jumping
            {
                m_rb.velocity = new Vector2(m_playerSide * speed, jumpForce);
                // jumpEffect
                PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
                jumpController.JumpExhaustion(20);
                audioManager.PlayJumpAudio();
            }
            m_touchJump = false;
           // Debug.Log(m_rb.velocity);
        }

        void Flip()
        {
            m_facingRight = !m_facingRight;
            m_playerSide *= -1;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        void CalculateSides()
        {
            if (m_facingRight)
                m_playerSide = 1f;
            else
                m_playerSide = -1f;

            // Flipping
            if (!m_facingRight && (moveInput > 0f || m_touchInput > 0))
                Flip();
            else if (m_facingRight && (moveInput < 0f || m_touchInput < 0))
                Flip();
        }

        public void JumpDashStatus(bool canJumpDash)
        {
            this.canJumpDash = canJumpDash;
        }

        public void DashStatus(bool canDash)
        {
            this.canDash = canDash;
            uIManager.UpdateDashImg(this.canDash);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        public void DashButtonInput()
        {
            m_DashBtnInput = true;
        }

        public void StopJumpPhysics()
        {
            m_rb.gravityScale = 1f;
            stopJumpPhysics = true;
        }
    }

    
}
