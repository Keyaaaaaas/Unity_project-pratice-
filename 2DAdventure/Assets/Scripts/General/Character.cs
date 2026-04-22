using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Character : MonoBehaviour
{
    [Header("属性")]
    public float maxHealth;
    public float currentHealth;
    public void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(Attack attacker)
    {
        currentHealth -= attacker.damage;

    }

    


}
