using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerMuerte : MonoBehaviour
{
    //GAMEOBJECTS
    public Image cooldown;
    //public  diedText;
    bool coolingDown;
    [SerializeField] float timer = 20f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (coolingDown == true)
        {
            cooldown.fillAmount -= 1f / timer * Time.deltaTime;
        }
    }
    public void MuertePlayerTimer()
    {
        coolingDown = true;
    }
}
