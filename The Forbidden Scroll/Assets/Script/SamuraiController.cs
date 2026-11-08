using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SamuraiController : MonoBehaviour
{
    //variables for movement
    [Header("movement")]
    public float normalSpeed = 5f;
    public float jumpForce = 5f;
    public float rotationSpeed = 15f;

    //variables for attack and health
    [Header("attack and health")]
    public int maxHp = 100;
    private int currentHp;

    public Image healthBarFill;

    private bool isGameOver = false;
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

        healthBarFill.fillAmount = 1f; 

        // keeps the player from falling over but still lets him turn around
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                            RigidbodyConstraints.FreezeRotationZ; 
    }

    void Update()
    {

        if(isGameOver)
        {
            if(Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            return;

        }
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(10);
        }

        // used for the vectors that we use to move
        float moveX = 0f;
        float moveZ = 0f;
        //checks if the Keys W,A,S,D or alternetavly the Arrowkeys are pressed for moving)
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ = -1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ = 1f;


        float currentSpeed = normalSpeed;

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
               //Samurai dreht sich in die Laufrichtung
               Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);

               Quaternion newRotation = Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
               );

                //Samurai bewegt sich in die Laufrichtung
                Vector3 movement = moveDir * currentSpeed * Time.deltaTime;
                rb.MovePosition(rb.position + movement);
            }
        }

        //you can only jump when you are not crouched 
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // attack with the left mouse button
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }


    //used for the attack
    void Attack()
    {
        Debug.Log("Samurai is attacking");
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
        if (currentHp <= 0) return;
        currentHp = Mathf.Max(0, currentHp - damage);
        healthBarFill.fillAmount = (float)currentHp / maxHp;
        Debug.Log("Samurai took damage: Current HP: " + currentHp);

        if (currentHp == 0)
        {
            GameOver();
        }
    }

    //for the Game over scene that is suppost to be played
    void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER! The Samurai died.");

        //Physikbewegung stoppen
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
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