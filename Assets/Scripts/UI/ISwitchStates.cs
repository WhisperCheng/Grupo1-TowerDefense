using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ISwitchStates : MonoBehaviour
{ 
    public interface ISwitchable
    {
        public void Activate();
        public void Deactivate();
    }

}
