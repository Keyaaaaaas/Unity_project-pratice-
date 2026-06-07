using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;
public class Character : MonoBehaviour
{
    [Header("属性")]
    public float maxHealth;
    public float currentHealth;
    [Header("受伤效果")]
    public float invulnerableDuration;
    private float invulnerableCounter; 
    public bool inInvulnerable;
    
    public UnityEvent<Transform> OnTakeDamage; // 受伤事件
    public UnityEvent OnDead; // 死亡事件

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(inInvulnerable)
        {
            invulnerableCounter -= Time.deltaTime; // 递减无敌持续时间
            if(invulnerableCounter <= 0)
            {
                inInvulnerable = false; // 无敌状态结束
            }
        }
    }

    public void TakeDamage(Attack attacker)
    {
        if(inInvulnerable)
        {
            return; // 如果处于无敌状态，直接返回
        }
        if(currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage; // 扣除生命值
            TriggerInvulnerable(); // 触发无敌状态
            OnTakeDamage?.Invoke(attacker.transform); // 触发受伤事件，传递攻击者的Transform

        }
        else
        {
            currentHealth = 0; // 生命值降至0
            OnDead?.Invoke(); // 触发死亡事件
        }

    }
    private void TriggerInvulnerable()
    {
        if(!inInvulnerable)
        {
            inInvulnerable = true;
            invulnerableCounter = invulnerableDuration; // 设置无敌持续时间

        }
    }

    


}
