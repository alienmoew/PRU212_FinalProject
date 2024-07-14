using System;
using Photon.Pun;
using UnityEngine;

public class HealthSystem : MonoBehaviourPunCallbacks
{
    public event EventHandler OnHealthChanged;
    private int health;
    private int healthMax;

    public HealthSystem(int healthMax)
    {
        this.healthMax = healthMax;
        health = healthMax;
    }

    public int GetHealth() { return health; }

    public int GetHealthMax() { return healthMax; }

    public void SetHealthMax(int healthMax)
    {
        this.healthMax = healthMax;
        if (health > healthMax) health = healthMax;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthPercent()
    {
        return (float)health / healthMax;
    }

    [PunRPC]
    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        if (health < 0) health = 0;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    [PunRPC]
    public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > healthMax) health = healthMax;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    [PunRPC]
    public void SetHealth(int healthAmount)
    {
        health = healthAmount;
        if (health > healthMax) health = healthMax;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }
}
