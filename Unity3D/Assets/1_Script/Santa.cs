using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Santa : MonoBehaviour
{
    [SerializeField] Player player;

    public void Attack()
    {
        player.AttackMonster();
    }
}
