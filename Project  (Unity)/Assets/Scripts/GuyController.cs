using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MathNet.Numerics.Distributions;

public class GuyController : MonoBehaviour
{

    public float jetpackForce = 10.0f;
    private Rigidbody2D playerRigidbody;
    public float forwardMovementSpeed = 3.0f;
    public Transform groundCheckTransform;
    private bool isGrounded;
    public LayerMask groundCheckLayerMask;
    private Animator guyAnimator;
    public ParticleSystem jetpack;
    private bool isDead = false;
    private uint masks = 0;
    public Text masksCollectedLabel;
    public Button restartButton;
    public AudioClip maskCollectSound;
    public AudioSource jetpackAudio;
    public AudioSource footstepsAudio;
    public ParallaxScroll parallax;
    public Normal norm = new Normal(5,2);

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        guyAnimator = GetComponent<Animator>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        bool jetpackActive = Input.GetButton("Fire1");

        jetpackActive = jetpackActive && !isDead;

        if (jetpackActive)
        {
            playerRigidbody.AddForce(new Vector2(0, jetpackForce));
        }

        if (!isDead)
        {
            Vector2 newVelocity = playerRigidbody.velocity;
            if (masks % 10 == 0)
            {
                forwardMovementSpeed = (float) norm.Sample();
            }
            newVelocity.x = forwardMovementSpeed;
            playerRigidbody.velocity = newVelocity;
        }
        UpdateGroundedStatus();
        AdjustJetpack(jetpackActive);
        if (isDead && isGrounded)
        {
            restartButton.gameObject.SetActive(true);
        }
        AdjustFootstepsAndJetpackSound(jetpackActive);
        parallax.offset = transform.position.x;
    }

    void UpdateGroundedStatus()
    {
        //1
        isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundCheckLayerMask);
        //2
        guyAnimator.SetBool("isGrounded", isGrounded);
    }

    void AdjustJetpack(bool jetpackActive)
    {
        var jetpackEmission = jetpack.emission;
        jetpackEmission.enabled = !isGrounded;
        if (jetpackActive)
        {
            jetpackEmission.rateOverTime = 300.0f;
            //isFlying = true;
        }
        else
        {
            jetpackEmission.rateOverTime = 50.0f;
           // isFlying = false;
        }
        guyAnimator.SetBool("isFlying", jetpackActive);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            if (collider.gameObject.CompareTag("Mask"))
            {
                CollectMask(collider);
            }
            else if (collider.gameObject.CompareTag("Desinfetante"))
            {
                CollectDesifetante(collider);
            }
            else if (collider.gameObject.CompareTag("Vaccine"))
            {
                CollectVaccine(collider);
            }
            else if (collider.gameObject.CompareTag("Cloud"))
            {
                HitByCloud(collider);
            }
            else
            {
                HitByLaser(collider);
            }
        }
    }

    void HitByLaser(Collider2D laserCollider)
    {
        if (!isDead)
        {
            AudioSource laserZap = laserCollider.gameObject.GetComponent<AudioSource>();
            laserZap.Play();
        }
        isDead = true;
        guyAnimator.SetBool("isDead", true);
    } 
    
    void HitByCloud(Collider2D cloudCollider)
    {
        isDead = true;
        guyAnimator.SetBool("isDead", true);
    }

    void CollectMask(Collider2D maskCollider)
    {
        masks++;
        masksCollectedLabel.text = masks.ToString();
        Destroy(maskCollider.gameObject);
        AudioSource.PlayClipAtPoint(maskCollectSound, transform.position);
    }
    
    void CollectDesifetante(Collider2D desinfetanteCollider)
    {
        masks += 5;
        masksCollectedLabel.text = masks.ToString();
        Destroy(desinfetanteCollider.gameObject);
    }
    
    void CollectVaccine(Collider2D vaccineCollider)
    {
        masks += 10;
        masksCollectedLabel.text = masks.ToString();
        Destroy(vaccineCollider.gameObject);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Covid");
    }

    void AdjustFootstepsAndJetpackSound(bool jetpackActive)
    {
        footstepsAudio.enabled = !isDead && isGrounded;
        jetpackAudio.enabled = !isDead && !isGrounded;
        if (jetpackActive)
        {
            jetpackAudio.volume = 1.0f;
        }
        else
        {
            jetpackAudio.volume = 0.5f;
        }
    }

}
