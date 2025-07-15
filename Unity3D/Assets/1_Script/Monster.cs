using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{

    [SerializeField] Animator anicon;

    public void Damaged()
    {
        anicon.SetTrigger("DAMAGED");
    }
}
