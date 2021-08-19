using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{
    private int maxHealth;
    private int currentHealth;

    public HealthSystem(int health = 0)
    {
        maxHealth = health;
        currentHealth = health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public void Damage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0)
            currentHealth = 0;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void HealMax()
    {
        currentHealth = maxHealth;
    }

    public void SetMaxHealth(int newMaxHealth, bool updateCurrentHealth)
    {
        maxHealth = newMaxHealth;

        if (updateCurrentHealth)
            currentHealth = newMaxHealth;
    }
}
