using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanges : MonoBehaviour
{
    public static void OpenTutorial()
    {
     
        SceneManager.LoadScene("TutorialScene");
    }

    public static void OpenLevel()
    {
        
        SceneManager.LoadScene("LevelOne");
    }

    public static void OpenAnimatic()
    {
        SceneManager.LoadScene("Animatica");
    }
    public static void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
