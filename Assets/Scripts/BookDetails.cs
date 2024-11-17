using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookDetails : MonoBehaviour
{
    // Fields for storing individual book details
    public string bookTitle;
    public string bookDescription;
    public Sprite bookImage;

    // Reference to the IndexManager in the scene
    private IndexManager indexManager;

    private void Start()
    {
        // Find the IndexManager in the scene (adjust if necessary)
        indexManager = FindObjectOfType<IndexManager>();
    }

    // Function called when the book is clicked
    public void OnBookClicked()
    {
        if (indexManager != null)
        {
            // Pass the stored data to the IndexManager
            indexManager.ShowBookDetails(bookTitle, bookDescription, bookImage);
        }
    }
}
