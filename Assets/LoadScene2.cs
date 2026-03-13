using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene2 : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("Scene2", LoadSceneMode.Additive);
    }
}
