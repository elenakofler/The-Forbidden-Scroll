using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NinjaController : MonoBehaviour
{
    //variables for movement and for stealth
    [Header("movement")]
    public float normalSpeed = 5f;
    public float crouchSpeed = 2.2f;
    public float jumpForce = 5f;
    private bool isCrouching = false;
    public float rotationSpeed = 15f;
    private bool isGrounded = true;

    //variables for attack and health
    [Header("attack and health")]
    public int maxHp = 100;
    private int currentHp;
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer;

    //a bool tell tells us if the Scroll has been pickt up, in this case it is fals because it hasn't been pickt up yet
    [Header("win condition")]
    private bool hasScroll = false;

    //for the ridgidbody (for physics) and for the capsulecollider (is importent for the collison box)
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;


    //initializes the komponents and the basics needed for the game
    void Start()
    {
        //gets components for Rigidbody and CapsuleCollider
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        //sets the current HP at the start as the max HP
        currentHp = maxHp; 

        // keeps the player from falling over
        rb.freezeRotation = true; 
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // used for the vectors that we use to move
        float moveX = 0f;
        float moveZ = 0f;
        //checks if the Keys W,A,S,D or alternetavly the Arrowkeys are pressed for moving)
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ = -1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ = 1f;

        //Crouching when the player presses the shift key
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            ToggleCrouch();
        }

        //Slows down the player when the player is crouched
        float currentSpeed = isCrouching ? crouchSpeed : normalSpeed;


        //this is for the movement relative to the camera, used to make the player move into the direction that they are looking
       if (Camera.main != null)
        {
            Transform camTransform = Camera.main.transform;

            Vector3 camForward = camTransform.forward;
            Vector3 camRight = camTransform.right;

            camForward.y = 0f; //so the player doesn't go into the sky
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();
            Vector3 moveDir = (camForward * moveZ + camRight * moveX).normalized;

            //used for the rotation of the camera
            if (moveDir.sqrMagnitude > 0.01f)
            {
               
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                
                Vector3 movement = moveDir * currentSpeed * Time.deltaTime;
                rb.MovePosition(rb.position + movement);
            }
        }

        //you can only jump when you are not crouched and when you are on the ground
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isCrouching && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; //after jumping it is automatically set to false so the player cant jump again once they are in the air
        }


        // attack with the left mouse button
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }


    //changes the hight of the character --> important for the hidding mechnic so the player fits behind/ underneath smaller hurdles
    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (capsuleCollider != null)
        {
            if (isCrouching)
            {
                capsuleCollider.height = 1.0f;
                capsuleCollider.center = new Vector3(0, -0.5f, 0);
                Debug.Log("Ninja is crouching (slowed down)");
            }
            else
            {
                capsuleCollider.height = 2.0f;
                capsuleCollider.center = new Vector3(0, 0, 0);
                Debug.Log("Ninja is standing up.");
            }
        }
    }

    //for checking if the player is standing on the floor
    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            //checks if the player is standing on the ground, stopps the player from jumping up the walls 
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    //once the player is in the air the bool is set to false
    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    //used for the attack
    void Attack()
    {
        Debug.Log("Ninja is attacking");
        if (attackPoint != null)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
            foreach (Collider enemy in hitEnemies)
            {
                Debug.Log("Enemy was hit " + enemy.name);
            }
        }
    }

    //the damage calculation for the damage that the player takes, 
    //and if the HP sink to 0 its GameOver
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log("Ninja took damage: Current HP: " + currentHp);

        if (currentHp <= 0)
        {
            GameOver();
        }
    }

    //for the Game over scene that is suppost to be played
    void GameOver()
    {
        Debug.Log("GAME OVER! The Ninja died.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //to collect the scroll 
    public void CollectScroll()
    {
        hasScroll = true;
        Debug.Log("VICTORY! The scroll has been recoverd!");
    }

    //shows the attack range of the enemie so it is easier to use in the editor mode
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}