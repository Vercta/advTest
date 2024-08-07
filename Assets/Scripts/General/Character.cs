using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public double maxHealth;
    public double currentHealth;
    public double healthRecoverSpeed;
    public double maxPower;
    public double currentPower;
    public double powerRecoverSpeed;

    [Header("受伤保护")]
    public double invulnerableDuration;
    private double invulnerableCounter;
    public bool invulnerable;

    #region Unity.Event
    [Header("持久化订阅")]
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    #endregion

    private void Start()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;

        // while Game Start: 自动填充PlayerStatBar
        OnHealthChange?.Invoke(this);
    }

    public void Update()
    {
        if(invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;

            if(invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }

        // Power恢复
        if (currentPower < maxPower)
        {
            currentPower += Time.deltaTime*powerRecoverSpeed;
        }
        
        // Health恢复
        if (currentHealth < maxHealth)
        {
            currentHealth += Time.deltaTime*healthRecoverSpeed;
        }

    }

    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
        {
            return;
        }
        // calc to currentHP
        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();

            // while Hurt
            OnTakeDamage?.Invoke(attacker.transform);

        }
        else
        {
            // while char.Death
            currentHealth = 0;
            OnDie?.Invoke();
        }

        // send 血量变化数值
        OnHealthChange?.Invoke(this);
        
    }

    // 受伤无敌状态
    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}
