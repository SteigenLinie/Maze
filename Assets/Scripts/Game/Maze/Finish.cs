using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    public GameObject WallLeft;
    public GameObject WallRight;
    public GameObject WallBottom;
    public GameObject WallUpper;

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("OnTriggerEnter2D");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
