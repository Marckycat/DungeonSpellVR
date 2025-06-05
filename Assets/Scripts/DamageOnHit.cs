using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    Fire,
    Ice,
    Air,
    Lighting
}

public interface IStatusEffect
{
    void ApplyFire(float tickDamage, float duration);
    void ApplySlow(float slowMultiplier, float duration);
    void ApplyStun(float duration);
    void ApplyPush(Vector3 force);
}

public class DamageOnHit : MonoBehaviour
{
    public float damageAmount;
    public float destroyDelay = 0.05f;
    private float finalDamage = 0f;

    public ElementType elementType;

    public float fireDureation = 3f;
    public float fireTickDamage = 5f;

    public float iceSlowAmount = 0.5f;
    public float iceDuration = 2f;

    public float airPushForce = 10f;

    public float lightingStunDurtation = 2f;


    private void OnCollisionEnter(Collision collision)
    {
        //Solo aplicar daño si el Rigidbody ya fue lanzado
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        IDamage damageable = collision.collider.GetComponent<IDamage>();
        if (damageable != null)
            damageable.TakeDamage(finalDamage);

        IStatusEffect statusEffect = collision.collider.GetComponent<IStatusEffect>();
        if(statusEffect != null)
        {
            switch (elementType)
            {
                case ElementType.Fire:
                    statusEffect.ApplyFire(fireTickDamage, fireDureation);
                    break;

                case ElementType.Ice:
                    statusEffect.ApplySlow(iceSlowAmount, iceDuration);
                    break;
                case ElementType.Air:
                    Rigidbody targetRb = collision.collider.GetComponent<Rigidbody>();
                    if (targetRb != null)
                    {
                        Vector3 pushDir = collision.contacts[0].normal * -1f;
                        targetRb.AddForce(pushDir * airPushForce, ForceMode.Impulse);
                    }
                    break;
                case ElementType.Lighting:
                    statusEffect.ApplyStun(lightingStunDurtation);
                    break;
            }
        }

        //Desactivar el objeto despues de un momento
        StartCoroutine(DisableAfterDelay());
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        gameObject.SetActive(false);
    }

    public void SetDamageMultiplier(float multiplier)
    {
        finalDamage = damageAmount * multiplier;
    }
}
