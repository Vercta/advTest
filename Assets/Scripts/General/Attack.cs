using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("攻击参数")]
    public int damage;
    public double attackRange;
    public float attackRate;

    private void OnTriggerStay2D(Collider2D other)
    {
        // use ? to secu
        other.GetComponent<Character>()?.TakeDamage(this);
    }
}
