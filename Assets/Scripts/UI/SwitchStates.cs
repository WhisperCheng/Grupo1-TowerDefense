using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchStates : MonoBehaviour
{ 
    public interface ISwitchable
    {
        public bool IsActive { get; }
        public void Activate();
        public void Deactivate();
    }

   
    //Activate activa el elemento llamado por [Serialize Field]
    void Start()
    {
        
    }

    //Deactivate desactiva el elemento llamado por [Serialize Field]
    void Update()
    {
        
    }
}
