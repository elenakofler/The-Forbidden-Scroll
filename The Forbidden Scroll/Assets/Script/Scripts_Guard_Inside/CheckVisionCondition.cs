using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

[System.Serializable, NodeDescription(name: "Check Player in Vision", story: "Player is in vision of Self")]
public class CheckVisionCondition : Unity.Behavior.Condition
{
    public float viewDistance = 12.0f; // Maximale Sichtweite
    public float viewAngle = 90.0f;    // Sichtwinkel in Grad (z.B. 90 = 45 Grad nach links und rechts)

    public override bool IsTrue()
    {
        // 1. Finde den Spieler
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return false;

        // 2. Finde den Agenten (die Wache) über den Blackboard-Eintrag oder NavMeshAgent
        NavMeshAgent agent = Object.FindAnyObjectByType<NavMeshAgent>();
        if (agent == null) return false;

        Transform guardTransform = agent.transform;
        Vector3 directionToPlayer = playerObj.transform.position - guardTransform.position;
        float distance = directionToPlayer.magnitude;

        // 3. Prüfen, ob der Spieler in Reichweite ist
        if (distance <= viewDistance)
        {
            // 4. Prüfen, ob der Spieler im Sichtwinkel (vor der Wache) ist
            Vector3 forwardWithoutY = new Vector3(guardTransform.forward.x, 0, guardTransform.forward.z).normalized;
            Vector3 directionWithoutY = new Vector3(directionToPlayer.x, 0, directionToPlayer.z).normalized;

            float angle = Vector3.Angle(forwardWithoutY, directionWithoutY);

            if (angle <= viewAngle / 2f)
            {
                // Optional: Raycast/Linecast, damit Wände das Sichtfeld blockieren
                if (!Physics.Linecast(guardTransform.position + Vector3.up * 1.0f, playerObj.transform.position + Vector3.up * 1.0f))
                {
                    return true; // Spieler ist im Sichtfeld und nicht hinter einer Wand!
                }
            }
        }

        return false;
    }
}