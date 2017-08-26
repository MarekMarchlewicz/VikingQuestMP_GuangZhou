using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(HealthIndicator))]
public abstract class ICharacter : NetworkBehaviour
{
    public event System.Action<int> OnTakenDamage;

    public virtual void TakeDamage(int damage)
    {
        if (OnTakenDamage != null)
        {
            OnTakenDamage(damage);
        }
    }

    [ClientRpc]
    public void Rpc_HealthChanged(int newHealth)
    {
        GetComponent<HealthIndicator>().UpdateHealth(newHealth);
    }

    [ClientRpc]
    public void Rpc_Die()
    {
        OnDied();
    }

	protected virtual void OnDied() {}
}
