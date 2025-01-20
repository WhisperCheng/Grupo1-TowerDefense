using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanges : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenTutorial()
    {
        //Nombre de la escena que sea el tutorial
        SceneManager.LoadScene("TutorialScene");
    }
    public void OpenLevel()
    {
        //Nombre de la escena que sea el nivel 1
        SceneManager.LoadScene("LevelOneScene");
    }
}
