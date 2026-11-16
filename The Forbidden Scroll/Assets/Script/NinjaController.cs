using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NinjaController : MonoBehaviour
{
    // Variables for movement and stealth
    [Header("Movement")]
    public float normalSpeed = 5f;
    public float crouchSpeed = 2.2f;
    public float jumpForce = 5f;
    private bool isCrouching = false;
    public float rotationSpeed = 15f;
    private bool isGrounded = true;

    // Variables for attack and health
    [Header("Attack and Health")]
    public int maxHp = 100;
    private int currentHp;
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer;
    public int attackDamage = 4; 
    public Image healthBarFill;

    private bool isGameOver = false;

    // Win condition variable
    [Header("Win Condition")]
    private bool hasScroll = false;

    // Components for physics and collision
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentHp = maxHp; 

        // Keeps the player from falling over
        rb.freezeRotation = true; 
    }

    void Update()
    {
        if (Keyboard.current == null || isGameOver) return;

        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ = -1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ = 1f;

        // Crouching when the player presses the shift key
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            ToggleCrouch();
        }

        float currentSpeed = isCrouching ? crouchSpeed : normalSpeed;

        // Movement relative to the camera direction
        if (Camera.main != null)
        {
            Transform camTransform = Camera.main.transform;

            Vector3 camForward = camTransform.forward;
            Vector3 camRight = camTransform.right;

            camForward.y = 0f; // Keep player level
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = (camForward * moveZ + camRight * moveX).normalized;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                Vector3 movement = moveDir * currentSpeed * Time.deltaTime;
                rb.MovePosition(rb.position + movement);
            }
        }

        // Jumping
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isCrouching && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Attacking with the left mouse button
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (capsuleCollider != null)
        {
            if (isCrouching)
            {
                capsuleCollider.height = 1.0f;
                capsuleCollider.center = new Vector3(0, -0.5f, 0);
            }
            else
            {
                capsuleCollider.height = 2.0f;
                capsuleCollider.center = new Vector3(0, 0, 0);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    void Attack()
    {
        // Use attackPoint position if assigned, otherwise use transform.position
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position;
        Collider[] hitObjects = Physics.OverlapSphere(origin, attackRange, enemyLayer);

        foreach (Collider col in hitObjects)
        {
            EnemyHealth guard = col.GetComponent<EnemyHealth>();
            if (guard != null && !guard.IsDead())
            {
                guard.TakeDamage(attackDamage);
                Debug.Log("Ninja is attacking");
                break; 
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;

        currentHp = Mathf.Max(0, currentHp - damage);
        
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHp / maxHp;
        }

        Debug.Log("Ninja took damage: Current HP: " + currentHp);

        if (currentHp == 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER! The Ninja died.");

        PlayerPrefs.SetString("LastLevel", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        SceneManager.LoadScene("GameOverScene");
    }

    public void CollectScroll()
    {
        hasScroll = true;
        Debug.Log("VICTORY! The scroll has been recovered! HasScroll status: " + hasScroll);
        
        // Example win condition logic: SceneManager.LoadScene("WinScene");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}