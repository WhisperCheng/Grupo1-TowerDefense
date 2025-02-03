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
    [SerializeField] protected bool _coolingDown;
    [SerializeField] float timer = 20f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_coolingDown == true)
        {
            cooldown.fillAmount -= 1f / timer * Time.deltaTime;
            GameUIManager.Instance.otherPanelActive = true;
        }
        if (cooldown.fillAmount <= 0)
        {
            _coolingDown = false;
            GameUIManager.Instance.otherPanelActive = false;
        }
    }
    public void ActivateRespawnTimer()
    {
        panelDied.SetActive(true);
        _coolingDown = true;
        cooldown.fillAmount = 1; // Reset del relleno de la imagen
        StartCoroutine(ReespawnPlayerAfterWaiting());
        Player.playerModel.SetActive(false);
        Player.GetComponent<CharacterController>().enabled = false;
    }

    private IEnumerator ReespawnPlayerAfterWaiting()
    {
        yield return new WaitForSeconds(timer);
        ReturnPlayer();
        yield return null;
    }

    private void ReturnPlayer()
    {
        Player.ReSpawn(respawnPoint);
        Player.GetComponent<CharacterController>().enabled = true;
        Player.playerModel.SetActive(true);
        panelDied.SetActive(false);
    }
}
