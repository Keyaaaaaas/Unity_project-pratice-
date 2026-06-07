using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roar : Enemy
{
    override public void Move()
    {
        base.Move();
        anim.SetBool("Walk", true);
    }    
}
