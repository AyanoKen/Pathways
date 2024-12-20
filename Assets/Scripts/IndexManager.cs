using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class IndexManager : MonoBehaviour
{
    // Reference to the parent object containing all the book buttons
    [SerializeField] private GameObject booksParent;

    // Reference to the "Book" GameObject which contains book details
    [SerializeField] private GameObject bookDetails;

    // UI elements inside the "Book" GameObject
    [SerializeField] private TextMeshProUGUI bookTitle;
    [SerializeField] private TextMeshProUGUI bookDescription;
    [SerializeField] private Image bookImage;

    [SerializeField] private int bookCount;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowBookDetails(string title, string description, Sprite image, int bookIndex)
    {
        // Update the book details UI elements
        bookTitle.text = title;
        bookDescription.text = description;
        bookImage.sprite = image;
        bookCount = bookIndex;
        Debug.Log(bookCount);

        // Disable the "Books" GameObject
        booksParent.SetActive(false);

        // Enable the "Book" GameObject
        bookDetails.SetActive(true);
    }

    public void ExitView()
    {
        booksParent.SetActive(true);
        bookDetails.SetActive(false);
    }

    public void StartLevel()
    {
        PlayerPrefs.SetInt("SelectedBookIndex", bookCount);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


}
