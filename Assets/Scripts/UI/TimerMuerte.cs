using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerMuerte : MonoBehaviour
{
    public Player Player { get; set; }
    public Transform respawnPoint;
    public static TimerMuerte instance { get; private set; }

    //GAMEOBJECTS
    public GameObject panelDied;
    public Image cooldown;
    public bool coolingDown;
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
        StartCoroutine(ReturnPlayerWaiting());
        Player.playerModel.SetActive(false);
        Player.GetComponent<CharacterController>().enabled = false;
    }

    private IEnumerator ReturnPlayerWaiting()
    {
        yield return new WaitForSeconds(timer);
        ReturnPlayer();
    }

    public void ReturnPlayer()
    {
        Player.ReSpawn(respawnPoint);
        Player.GetComponent<CharacterController>().enabled = true;
        Player.playerModel.SetActive(true);
        
        panelDied.SetActive(false);
    }
}
