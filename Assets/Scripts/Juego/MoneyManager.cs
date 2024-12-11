using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    int gemas = 50; //número de gemas que tenga inicialmente X
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void QuitarDineroRosaEspinas()
    {
        gemas -= 5; //num X según lo que se quiera quitar por cada rosa que se ponga
    }
    public void QuitarDineroPlantaCarnovira()
    {
        gemas -= 10; //num X según lo que se quiera quitar por cada carnivora que se ponga
    }
    public void QuitarDineroTajoVenenoso()
    {
        gemas -= 20; //num X según lo que se quiera quitar por cada tajo que se ponga
    }
    public void QuitarDineroMejoras()
    {
        gemas -= 25; //num X según se quite para las mejoras de plantas
    }
}
