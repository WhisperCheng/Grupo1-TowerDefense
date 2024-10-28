using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoDesaparicion : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem particulas;
    public GameObject enemigo;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            enemigo.SetActive(false);
            particulas.Play();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            enemigo.SetActive(true); 
            
        }
    }
}
