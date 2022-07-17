using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TableDungeon
{
    public class MainMenuScript : MonoBehaviour
    {
        public string sceneName = "MainScene";
        
        public void PlayButton()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
