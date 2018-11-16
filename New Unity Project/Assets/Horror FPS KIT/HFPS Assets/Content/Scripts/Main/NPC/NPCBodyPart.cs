using UnityEngine;

public class NPCBodyPart : MonoBehaviour {

    [HideInInspector]
    public NPCHealth health;

    public void ApplyDamage(int damage)
    {
        health.Damage(damage);
    }
}
