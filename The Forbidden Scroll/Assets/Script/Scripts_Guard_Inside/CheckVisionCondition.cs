using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

[System.Serializable, NodeDescription("Check Player in Vision", "Player is in vision of Self")]
public class CheckVisionCondition : Unity.Behavior.Condition
{
    public float viewDistance = 12.0f; // Maximum visibility range
    public float viewAngle = 90.0f;    // Viewing angle in degrees

    public override bool IsTrue()
    {
        // 1. Finding the Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return false;

        // 2. Finding the specific Guard (Fix: Use 'GameObject' instead of FindAnyObjectByType)
        Transform guardTransform = null;
        if (GameObject != null)
        {
            NavMeshAgent agent = GameObject.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                guardTransform = agent.transform;
            }
            else
            {
                guardTransform = GameObject.transform;
            }
        }

        // Fallback, falls das GameObject im Behavior Graph nicht zugewiesen ist
        if (guardTransform == null)
        {
            NavMeshAgent fallbackAgent = Object.FindAnyObjectByType<NavMeshAgent>();
            if (fallbackAgent == null) return false;
            guardTransform = fallbackAgent.transform;
        }

        Vector3 directionToPlayer = playerObj.transform.position - guardTransform.position;
        float distance = directionToPlayer.magnitude;

        // 3. Checking, if the player is within range
        if (distance <= viewDistance)
        {
            // 4. Checking, if the player is within the field of view
            Vector3 forwardWithoutY = new Vector3(guardTransform.forward.x, 0, guardTransform.forward.z).normalized;
            Vector3 directionWithoutY = new Vector3(directionToPlayer.x, 0, directionToPlayer.z).normalized;

            float angle = Vector3.Angle(forwardWithoutY, directionWithoutY);

            if (angle <= viewAngle / 2f)
            {
                if (!Physics.Linecast(guardTransform.position + Vector3.up * 1.0f, playerObj.transform.position + Vector3.up * 1.0f))
                {
                    return true;
                }
            }
        }

        return false;
    }
}