using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;


public class CaballeroIA : EnemigoIA
{

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        WhileWalking();
    }
}
