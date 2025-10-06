using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Click : MonoBehaviour
{
    public void SceneChange(string nextSceneName)
    {
        SceneManager.LoadScene(nextSceneName);
    }
    public void OverlayOpen(string overlayName){
        
    }
}
    