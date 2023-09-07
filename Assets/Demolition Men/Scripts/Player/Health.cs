using System.Collections;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    private int currentHealth;
    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);

            if (currentHealth <= 0)
            {
                OnDeathEvent?.Invoke();
                OnDeath();
                StopAllCoroutines();
            }

        }
    }

    public delegate void Death();
    public event Death OnDeathEvent;  // To be 'subscribed' to to handle external death events.

    protected virtual void OnEnable() => CurrentHealth = maxHealth;

    public void ResetHealth() => CurrentHealth = maxHealth;

    public void HealOverTime(float healIntervalSeconds, int healAmount)
    {
        _ = StartCoroutine(AddHealthOverTimeCR(healIntervalSeconds, healAmount));

        IEnumerator AddHealthOverTimeCR(float healIntervalSeconds, int healAmount)
        {
            WaitForSeconds interval = new(healIntervalSeconds);

            while (healAmount > 0)
            {
                CurrentHealth += healAmount;
                healAmount--;

                yield return interval;
            }
        }
    }

    public abstract void OnDeath();
}
