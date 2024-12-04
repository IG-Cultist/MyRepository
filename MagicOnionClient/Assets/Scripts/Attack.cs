using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using UnityEngine;

public class Attack : MonoBehaviour
{
    Player playerScript;

    void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
    }
}
