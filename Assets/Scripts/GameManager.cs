using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class GameManager : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] private Button choiceButton1;
    [SerializeField] private Button choiceButton2;
    [SerializeField] private Button choiceButton3;
    [SerializeField] private Button choiceButton4;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private Image storyImage;


    [Header("API Settings")]
    private int bookIndex;
    private string apiKey = "";
    private string chatUrl = "https://api.openai.com/v1/chat/completions";
    private string imageGenerationUrl = "https://api.openai.com/v1/images/generations";

    private string[] chatPrompts = {
        "Now let's create a Make your Own Adventure book based on the following theme: You awaken in a forest shrouded in eternal twilight, your memory fractured like shards of broken glass. All you know is that your name is whispered in fear by the wind, and a crumbling map clutched in your hand bears the name Grimore Hollow. The map leads to a cursed land where time has stopped, monsters roam freely, and a dark sorcerer guards a relic known as the Crown of Eternal Night—an artifact said to grant its wielder unparalleled power but at a devastating cost. Your journey will take you through haunted ruins, labyrinthine caverns, and cursed villages where whispers of the sorcerer’s origins and the secrets of your own past linger in every shadow. Along the way, you must battle monstrous foes, solve ancient riddles, and make life-altering choices. Will you succumb to the darkness, or will you unearth the light hidden within the hollow heart? Each choice brings you closer to your destiny—or your doom.The fate of Grimore Hollow and the truth about who you are rests in your hands. At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt along with artstyle guide I can use with image generation to visualize the scene. Make sure that the artstyle stays consistent in the subsequent image prompts, this is very important. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "second"
    };

    private string[] artstyle = {
        "Dark and surreal fantasy, with a painterly, dreamlike quality. Colors are rich and brooding, emphasizing deep blacks, crimson reds, shadowy grays, and muted golds, contrasted with ethereal glows of pale silver and fiery orange.",
        "second"
    };

    private List<Dictionary<string, string>> chatHistory = new List<Dictionary<string, string>>();

    void Start()
    {
        bookIndex = PlayerPrefs.GetInt("SelectedBookIndex", 1) - 1;
        Debug.Log("Current Selected Book is: " + bookIndex);

        chatHistory.Add(new Dictionary<string, string>
        {
            { "role", "system" },
            { "content", chatPrompts[bookIndex] }
        });

        StartCoroutine(FetchStory(chatPrompts[bookIndex]));
    }

    private IEnumerator FetchStory(string prompt)
    {
        string jsonBody = JObject.FromObject(new
        {
            model = "gpt-4",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = prompt
                }
            },
            max_tokens = 3000,
            temperature = 0.7
        }).ToString();

        // Create a UnityWebRequest with the chat endpoint
        UnityWebRequest chatRequest = new UnityWebRequest(chatUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        chatRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        chatRequest.downloadHandler = new DownloadHandlerBuffer();
        chatRequest.SetRequestHeader("Content-Type", "application/json");
        chatRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Send the request and wait for the response
        yield return chatRequest.SendWebRequest();

        if (chatRequest.result == UnityWebRequest.Result.ConnectionError || chatRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + chatRequest.error);
        }
        else
        {
            // Parse the JSON response
            string responseText = chatRequest.downloadHandler.text;
            Debug.Log("Chat Response: " + responseText);

            JObject jsonResponse = JObject.Parse(responseText);
            string storyContent = jsonResponse["choices"][0]["message"]["content"].ToString().Trim();

            chatHistory.Add(new Dictionary<string, string>
            {
                { "role", "assistant" },
                { "content", storyContent }
            });

            // Update UI with story content
            UpdateStory(storyContent);

            
            string imagePrompt = ExtractImagePrompt(storyContent);
            Debug.Log("Initial Image Prompt is: " + imagePrompt);
            if (!string.IsNullOrEmpty(imagePrompt))
            {
                imagePrompt = imagePrompt + artstyle[bookIndex];
                StartCoroutine(GenerateImage(imagePrompt));
            }
        }
    }

    // Function to update the story text and choices
    private void UpdateStory(string storyContent)
    {
        // Split the response into lines
        string[] storyLines = storyContent.Split('\n');

        // Separate narrative and choices
        List<string> narrativeLines = new List<string>();
        List<string> choices = new List<string>();

        // Iterate through each line
        foreach (string line in storyLines)
        {
            // Check if the line starts with a number followed by a period (e.g., "1." or "2.")
            if (line.Trim().Length > 1 && char.IsDigit(line.Trim()[0]) && line.Trim()[1] == '.')
            {
                choices.Add(line.Trim()); // Treat it as a choice
            }
            else
            {
                narrativeLines.Add(line); // Otherwise, it's part of the story narrative
            }
        }

        // Update story text with narrative
        //storyText.text = string.Join("\n", narrativeLines);
        storyText.text = storyLines[0];

        // Update choice buttons
        Button[] choiceButtons = { choiceButton1, choiceButton2, choiceButton3, choiceButton4 };
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];

                choiceButtons[i].interactable = false;

                // Remove previous listener to avoid stacking events
                choiceButtons[i].onClick.RemoveAllListeners();
                
                // Assign the HandleChoice method with the choice text
                string choiceText = choices[i]; // Store the choice text in a local variable to avoid closure issue
                choiceButtons[i].onClick.AddListener(() => HandleChoice(choiceText));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false); // Hide unused buttons
                choiceButtons[i].interactable = false;
            }
        }
    }

    // Function to extract image prompt from the story content
    private string ExtractImagePrompt(string storyContent)
    {
        // Assuming the image prompt is within square brackets at the end of the content
        int startIndex = storyContent.LastIndexOf('[');
        int endIndex = storyContent.LastIndexOf(']');

        if (startIndex >= 0 && endIndex > startIndex)
        {
            return storyContent.Substring(startIndex + 1, endIndex - startIndex - 1);
        }
        return null;
    }

    // Function to generate an image based on a prompt
    private IEnumerator GenerateImage(string prompt)
    {
        // Prepare JSON payload for image generation
        string jsonBody = JObject.FromObject(new
        {
            model = "dall-e-3",
            prompt = prompt,
            n = 1,
            size = "1024x1024"
        }).ToString();

        // Create a UnityWebRequest with the image generation endpoint
        UnityWebRequest imageRequest = new UnityWebRequest(imageGenerationUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        imageRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        imageRequest.downloadHandler = new DownloadHandlerBuffer();
        imageRequest.SetRequestHeader("Content-Type", "application/json");
        imageRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Send the request and wait for the response
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result == UnityWebRequest.Result.ConnectionError || imageRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error generating image: " + imageRequest.error);
        }
        else
        {
            // Parse the JSON response
            string responseText = imageRequest.downloadHandler.text;
            Debug.Log("Image Response: " + responseText);

            JObject jsonResponse = JObject.Parse(responseText);
            string imageUrl = jsonResponse["data"][0]["url"].ToString();

            // Use the image URL for your image element if needed
            Debug.Log("Generated Image URL: " + imageUrl);

            StartCoroutine(DownloadImage(imageUrl));
        }
    }

    private IEnumerator DownloadImage(string imageUrl)
    {
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(imageUrl);

        // Send the request and wait for the response
        yield return textureRequest.SendWebRequest();

        if (textureRequest.result == UnityWebRequest.Result.ConnectionError || textureRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error downloading image: " + textureRequest.error);
        }
        else
        {
            // Get the downloaded texture
            Texture2D downloadedTexture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;

            // Apply the texture to the Image UI component
            storyImage.sprite = Sprite.Create(downloadedTexture, new Rect(0, 0, downloadedTexture.width, downloadedTexture.height), new Vector2(0.5f, 0.5f));
            storyImage.preserveAspect = true; // Maintain the aspect ratio of the image

            SetButtonsInteractable(true);
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        Button[] choiceButtons = { choiceButton1, choiceButton2, choiceButton3, choiceButton4 };
        foreach (Button button in choiceButtons)
        {
            button.interactable = interactable;
        }
    }

    public void HandleChoice(string choiceText)
    {
        // Add the selected choice to chat history as user input
        chatHistory.Add(new Dictionary<string, string>
        {
            { "role", "user" },
            { "content", choiceText }
        });

        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        choiceButton3.gameObject.SetActive(false);
        choiceButton4.gameObject.SetActive(false);

        storyText.text = "Loading the story, please wait!";

        // Fetch the next part of the story
        StartCoroutine(FetchStoryContinuation());
    }

    private IEnumerator FetchStoryContinuation()
    {
        // Prepare the JSON body using chat history
        string jsonBody = JObject.FromObject(new
        {
            model = "gpt-4",
            messages = chatHistory.ToArray(),
            max_tokens = 3000,
            temperature = 0.7
        }).ToString();

        // Create a UnityWebRequest with the chat endpoint
        UnityWebRequest chatRequest = new UnityWebRequest(chatUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        chatRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        chatRequest.downloadHandler = new DownloadHandlerBuffer();
        chatRequest.SetRequestHeader("Content-Type", "application/json");
        chatRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Send the request and wait for the response
        yield return chatRequest.SendWebRequest();

        if (chatRequest.result == UnityWebRequest.Result.ConnectionError || chatRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + chatRequest.error);
        }
        else
        {
            // Parse the JSON response
            string responseText = chatRequest.downloadHandler.text;
            Debug.Log("Chat Response: " + responseText);

            JObject jsonResponse = JObject.Parse(responseText);
            string storyContent = jsonResponse["choices"][0]["message"]["content"].ToString().Trim();

            // Update chat history with the assistant's response
            chatHistory.Add(new Dictionary<string, string>
            {
                { "role", "assistant" },
                { "content", storyContent }
            });

            // Update UI with the new story content
            UpdateStory(storyContent);

            string imagePrompt = ExtractImagePrompt(storyContent);
            Debug.Log("Image Prompt is: " + imagePrompt);
            if (!string.IsNullOrEmpty(imagePrompt))
            {
                imagePrompt = imagePrompt + artstyle[bookIndex];
                StartCoroutine(GenerateImage(imagePrompt));
            }
        }
    }
}
