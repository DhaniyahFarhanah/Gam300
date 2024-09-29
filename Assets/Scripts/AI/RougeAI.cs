using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RougeAI : BaseAI
{
    [Header("Rouge AI")]
    public bool slowWhenAvoiding = true;
    public bool slowWhenTurning = true;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        base.Init();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetPosition = player.transform.position;
        EngineUpdate();
    }
}
