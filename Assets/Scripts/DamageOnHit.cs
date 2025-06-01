using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnHit : MonoBehaviour
{
    public float damageAmount = 25f;
    public float destroyDelay = 0.05f;

    private void OnCollisionEnter(Collision collision)
    {
        //Solo aplicar daño si el Rigidbody ya fue lanzado
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null || rb.isKinematic) return;

        IDamage damageable = collision.collider.GetComponent<IDamage>();
        if (damageable != null)
            damageable.TakeDamage(damageAmount);

        // Desactivar o devolver al pool después del impacto
        // gameObject.SetActive(false); // Si no usas pool
        // o bien:
        // StartCoroutine(...) // Si usas el sistema de pool con retorno

        // Opcional: destruir si no lo vas a reutilizar
        // Destroy(gameObject);

        //Desactivar el objeto despues de un momento
        StartCoroutine(DisableAfterDelay());
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        gameObject.SetActive(false);
    }
}
