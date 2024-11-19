using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int bookIndex;
    void Start()
    {
        bookIndex = PlayerPrefs.GetInt("SelectedBookIndex", 1);
        Debug.Log("Current Selected Book is: " + bookIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
