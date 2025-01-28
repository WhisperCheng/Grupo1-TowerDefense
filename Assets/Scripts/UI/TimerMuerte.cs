using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerMuerte : MonoBehaviour
{
    public static TimerMuerte instance { get; private set; }
    //GAMEOBJECTS
    public GameObject panelDied;
    public Image cooldown;
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
        if (cooldown.fillAmount <= 0)
        {
            ReturnPlayer();
            coolingDown = false;
        }
    }
    public void DiedPlayerTimer()
    {
        panelDied.SetActive(true);
        coolingDown = true;
    }
    public void ReturnPlayer()
    {
        panelDied.SetActive(false);
    }
}
