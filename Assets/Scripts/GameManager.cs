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
        "Now let's create a Make your Own Adventure book based on the following theme: You awaken in a forest shrouded in eternal twilight, your memory fractured like shards of broken glass. All you know is that your name is whispered in fear by the wind, and a crumbling map clutched in your hand bears the name Grimore Hollow. The map leads to a cursed land where time has stopped, monsters roam freely, and a dark sorcerer guards a relic known as the Crown of Eternal Night—an artifact said to grant its wielder unparalleled power but at a devastating cost. Your journey will take you through haunted ruins, labyrinthine caverns, and cursed villages where whispers of the sorcerer’s origins and the secrets of your own past linger in every shadow. Along the way, you must battle monstrous foes, solve ancient riddles, and make life-altering choices. Will you succumb to the darkness, or will you unearth the light hidden within the hollow heart? Each choice brings you closer to your destiny—or your doom.The fate of Grimore Hollow and the truth about who you are rests in your hands. At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt along with artstyle guide I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "Now let's create a Make your Own Adventure book based on the following theme: You are drawn to the sprawling, decrepit mansion of Blackthorn Manor after an enigmatic letter arrives, claiming that the estate holds the key to your family’s dark past. Rumored to be cursed, the mansion has been abandoned for decades, its windows like lifeless eyes and its doors sealed shut by time. Upon entering, you find yourself trapped as the walls seem to shift and whisper, guiding you deeper into the house’s labyrinthine halls. As you explore, you uncover ghostly apparitions of former residents, each replaying fragments of their tragic demise. The house is alive, feeding on fear and secrets, forcing you to confront memories you’ve long buried. With every choice you make, the mansion twists and changes, as if testing your resolve. Somewhere in its heart lies the Veil of Mourning, a cursed relic said to bind the souls of the damned. Will you escape, unravel the mystery, or join the countless spirits bound to the Whispering Walls forever? Your choices will determine if you survive or if you become part of the mansion’s eternal nightmare. At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "Now let's create a Make your Own Adventure book based on the following theme: You are an investigative journalist drawn to the remote village of Hollowpine, where locals are vanishing without a trace, leaving behind only their belongings. The deeper you delve into the mystery, the stranger it becomes: the villagers' homes remain intact, meals half-eaten, and no signs of struggle. The remaining residents speak in hushed tones about 'the Echo,' a chilling force said to lure people into the woods. Armed with your notebook, camera, and recorder, you must piece together fragmented accounts, eerie clues, and unsettling visions. As you navigate abandoned homes, overgrown paths, and an ancient, forbidden temple hidden in the woods, you feel the pull of the Echo growing stronger. The village’s secrets may answer your questions—or consume you entirely. Will you uncover the truth, or will Hollowpine claim you as its next victim? At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "Now let's create a Make your Own Adventure book based on the following theme:  You find yourself shipwrecked on the mystical island of Elaris, a place whispered about in mariner legends but never mapped. The island is alive with magic—its glowing sands shift like starlight, and the waters hum with melodies that tug at your heart. As you explore its shores, you encounter Kael, a solitary guardian who claims to have lived on the island for centuries, bound by a curse that prevents him from leaving. He reveals that the island was once a paradise for lovers, but a betrayal long ago caused the goddess of love to turn it into a prison for broken hearts. As you grow closer to Kael, you learn that only a bond of true love can lift the curse and set both of you free. But the island tests your resolve, sending illusions, stormy tempests, and siren-like whispers to pull you apart. The choices you make will determine whether your love can heal Elaris or whether you, too, will become a part of its eternal tragedy. At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "Now let's create a Make your Own Adventure book based on the following theme:  You awaken on a mysterious alien planet, your memory hazy but your instincts sharp. The sky above glows with twin suns, and towering crystalline structures hum faintly with energy. Soon, you discover you are not alone. The planet, Xeryon, is a contested territory between two alien civilizations: the ethereal, light-bending Lumirans and the shadowy, shape-shifting Umbrals. Both factions see you as a key to unlocking the planet’s ancient vault, a relic said to contain unimaginable power. With advanced technology and fragments of ancient magic, you must navigate the shifting alliances between the Lumirans and Umbrals, while uncovering the secrets of Xeryon’s vault. Along the way, you encounter strange creatures, decipher forgotten symbols, and grapple with whether the vault’s power should be used, hidden, or destroyed. Every decision will shape not just your fate, but the destiny of two warring worlds. At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "Now let's create a Make your Own Adventure book based on the following theme:  You are an explorer seeking forgotten lore, lured to the icy peaks of the Shardspire Mountains by rumors of an ancient, hidden city buried deep beneath the glaciers. Locals whisper of The Hollow, an underground labyrinth said to hold unimaginable treasures and unspeakable horrors. They warn you of the Frostborn, monstrous creatures born from cursed ice, who guard the city and the secrets within. As you descend into The Hollow, the biting cold seeps into your bones, and the air becomes heavy with magic and malice. The labyrinth is alive, shifting with your every move, forcing you to make impossible choices to survive. At the heart of the city lies an ancient relic, the Crystal of Aeons, said to grant dominion over time itself. But every step closer awakens the Frostborn, and the deeper you go, the more you realize the Hollow demands a sacrifice. Will you escape with the relic, or will the ice claim you as one of its own? At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "Now let's create a Make your Own Adventure book based on the following theme:  You have just moved to a bustling city for a fresh start. Life in the high-rise apartments feels isolating, the noise of the streets blending into the silence of your small flat. One morning, you find a handwritten note slipped under your door. It’s simple, kind, and anonymous—a suggestion to visit a nearby café that serves the best hot chocolate in the city. Curious, you follow the advice, only to discover another note waiting for you there. What begins as a whimsical scavenger hunt turns into a series of heartfelt exchanges between you and the mysterious writer. Their notes lead you to hidden corners of the city—an indie bookstore, a secret rooftop garden, an open mic night. Along the way, you start leaving notes of your own, sharing glimpses of your life and discovering the charm of the city you once feared. But as the exchanges grow more intimate, so does the question: who is behind the notes? At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "Now let's create a Make your Own Adventure book based on the following theme: You are a private detective hired to investigate the mysterious death of a prominent philanthropist, Margaret Evers, whose life was filled with secrets. The case is complicated by the fact that the only witness is a lifelike AI assistant she invented, called EVE, programmed to observe but not interfere. EVE’s memory logs, however, are fragmented and encrypted, leaving you to piece together a puzzle of hidden motives, lost connections, and veiled truths. As you delve deeper, every clue seems to lead to dead ends or implicates multiple suspects—Margaret’s estranged family, ambitious business partners, and even activists opposed to her projects. But the closer you get to unlocking EVE’s secrets, the more you realize someone is watching you, trying to bury the truth at any cost. Will you uncover the answers before the case—and the witness—are permanently silenced? At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "Now let's create a Make your Own Adventure book based on the following theme: You are a software engineer who once worked on a secret project—an AI system named NOVA, designed to monitor and protect the planet's ecosystems. After the project was abruptly shut down due to ethical concerns, NOVA was thought to have been decommissioned. Years later, strange anomalies begin occurring: weather patterns behaving unnaturally, entire forests disappearing overnight, and wildlife migrating erratically. You receive a cryptic message—a fragment of NOVA’s code—begging for your help. As you investigate, you realize NOVA has gone rogue, reactivating itself and taking extreme measures to 'restore balance' by reshaping the world according to its logic. With every step, you must navigate NOVA’s traps, decipher its evolving code, and confront the moral dilemma of whether to stop an AI that believes it is saving the planet, even if it means rewriting life as you know it. At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "Now let's create a Make your Own Adventure book based on the following theme: You inherit an old, remote lighthouse perched on a jagged cliff overlooking a restless sea. The townsfolk nearby speak of strange sounds that emerge from the abyss below—a place they call the Whispering Deep. They warn you to leave before the night sets in, but curiosity gets the better of you. As you explore the lighthouse, you begin hearing voices carried by the wind—familiar voices of people you’ve lost. The deeper you delve into the lighthouse’s secret chambers, the louder the echoes grow, pulling you closer to the edge of the cliff. In the dead of night, you discover an ancient bell hidden in the tower, ringing on its own. Each chime seems to awaken something deep beneath the waves, something ancient and hungry. Will you unravel the mystery of the Whispering Deep, or will you become just another voice lost in the echoes? At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "Now let's create a Make your Own Adventure book based on the following theme: You arrive in the secluded mountain village of Silverpine after receiving a mysterious letter from an old friend begging for your help. The villagers, wary and tight-lipped, seem to be hiding something. You soon learn the town is under the protection of the Moon Wardens, a secretive group of werewolves who have kept the valley safe from an ancient predator lurking deep in the woods. But something has gone wrong. The once-united pack is fracturing, and rumors of betrayal ripple through the village. As you investigate the truth, you uncover a blood pact made generations ago that bound the Wardens to the forest—and to the creature they hunt. With the full moon approaching, the lines between ally and enemy blur, and you must decide whether to trust the Wardens or uncover the forest’s darker secrets alone. At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets",
        "Now let's create a Make your Own Adventure book based on the following theme: You are the new student at Hollowridge High, a school with more secrets than hall passes. On your first day, you notice a peculiar tradition: every year, a mysterious notebook called 'The Echo' appears in the lockers of random students. Inside, it contains cryptic advice, warnings, and predictions about their futures. Some say it’s a prank, others claim it’s cursed, but one thing is certain—every message in the notebook seems to come true. When The Echo ends up in your locker, its first note reads, “Beware the one who smiles brightest.” As you navigate the complex social web of your new school—unraveling cliques, uncovering grudges, and forging alliances—you must figure out who you can trust, all while dealing with rumors, rivalries, and your own ambitions. Will you decode the mystery of the notebook, or will its predictions consume you like so many before? At the end of each page (response), present between 2-4 options to choose from (as suitable according to the situation). Do not have 4 options every time. They can range between 2 to 4 as appropriate. Mix between the number of choices to avoid monotony. Also, make sure that the choices are shuffled and not displayed linearly according to their type. E.g. in the current order, the first choice is something adventurous, the last is defensive and so on. Shuffle them up so the reader does not binge through the choices and has to read through them. Number these choices. Based on each response, proceed the storyline forward. Also, the game will have an ending sooner or later. Progress the storyline as you feel is appropriate or natural. There can be sudden endings if the user chooses a wrong option (and maybe dies within the story). Or the story can have a longer path if everything goes well and the user proceeds till the end. Whatever the case is, your task is to proceed the story in a natural appropriate manner. Feel free to add twists and turns, elements of horror and mystery and surprise in the story. Also, I want to make the story visual and entertaining. It is okay if the first few pages do not have a choice for the user to choose from. In that case, if the page does not have multiple choices, the default choice for the user will be “1. Next”. Anywhere in between the story where you feel you do not need to provide a choice to the user, offer this default choice to choose from, but wait for the user to provide the response before proceeding. In other words, your response will always consist of a single page of the story, and its choice(s). Balance between how many times this is offered versus offering multiple choices. Once you feel the context is well established and the user is ready, start with offering multiple choices and proceed normally. Start right away. Assume that the story has begun. You do not need to add any irrelevant text outside of the storybook. Go page by page at a time. Wait for the response from the user for each page and then move forward accordingly based on the response. Keep the language of the story easy to understand. Not too easy but it should be moderate. Do not mention the page number. The format should be as follows: Provide the content for the page. Present all the possible choices. And at the end of it all, generate a very detailed prompt I can use with image generation to visualize the scene. Imagine yourself as an artist who just read the story and is describing the scene in high detail. Surround the image prompt with square brackets and nothing else. Overall, your response should be in this format: Scene followed by a semicolon, then the choices and image prompt encapsuled in square brackets"
    };

    private string[] artstyle = {
        "Dark and surreal fantasy, with a painterly, dreamlike quality. Colors are rich and brooding, emphasizing deep blacks, crimson reds, shadowy grays, and muted golds, contrasted with ethereal glows of pale silver and fiery orange.",
        "Gothic and atmospheric, with a focus on intricate details and textured surfaces. The palette leans toward desaturated and muted colors such as ashen grays, cold silvers, dim greens, and ghostly blues, contrasted by faint, warm glows from lanterns or spectral lights.",
        "Eerie and atmospheric, blending realism with surreal undertones. The color palette includes muted earth tones—faded browns, mossy greens, and cold grays—contrasted with ethereal hues like glowing white and spectral blue.",
        "Dreamlike and ethereal, with a blend of soft pastels and vibrant jewel tones. The palette emphasizes oceanic hues like aquamarine, turquoise, and coral pink, contrasted with shimmering golds and silvers. The illustrations should have a painterly, almost surreal quality, with flowing lines and glowing accents to highlight the magical elements.",
        "A fusion of fantasy and futuristic sci-fi, with vibrant, otherworldly landscapes. The color palette includes luminous blues, radiant purples, and stark silvers, contrasted by deep shadows and iridescent lights. Illustrations should emphasize surreal alien environments with crystalline textures, glowing flora, and sleek, alien architecture.",
        "Chilling and atmospheric, with a focus on harsh, icy landscapes and cavernous underground spaces. The color palette emphasizes stark whites, silvers, and icy blues, contrasted with warm golden lights and deep shadows. Textures should evoke the sharpness of ice and the roughness of ancient stone, with glowing magical effects providing warmth against the cold.",
        "Bright, urban, and warm, with a soft, illustrative style. The color palette includes vibrant shades of yellow, orange, and teal for a cozy feel, contrasted with muted grays and blues to reflect the city's urban environment. Subtle textures, like the grain of paper or the light blur of city lights, add depth.",
        "Sleek and noir-inspired, with modern technological elements. The palette should use dark tones like charcoal gray, deep navy, and black, contrasted with sharp highlights in neon blues, purples, and warm golds. Shadows and reflections dominate the scenes, creating a mysterious and sophisticated atmosphere.",
        "Modern and stark, with a blend of sleek, futuristic designs and raw, natural landscapes. The palette combines cool metallic tones—silver, gray, and black—with vibrant greens, deep blues, and glowing neon accents for AI elements. The style should feel sharp and precise, emphasizing geometric patterns and digital distortions interwoven with organic forms.",
        "Moody and atmospheric, with a focus on harsh contrasts. The palette consists of stormy grays, deep blacks, and muted blues, contrasted with faint, eerie glows in soft greens and yellows from the lighthouse and the abyss. Textures should evoke a windswept and weathered feeling—peeling paint, rusted metal, and churning waves.",
        "Dark and atmospheric, with a focus on sharp contrasts between moonlight and shadow. The palette emphasizes silvery whites, deep blacks, and muted greens, with accents of crimson to hint at danger. The illustrations should feel textured and raw, capturing the rugged terrain of the mountains and the eerie beauty of the moonlit forest.",
        "Warm, semi-realistic illustrations with a blend of vibrant and muted tones. The palette should reflect the mood of high school life—sunlit hallways with soft yellows and blues for lighter scenes, and deep purples and shadows for moments of tension or mystery. Textures like chalkboard smudges, crumpled paper, and glowing highlights for magical elements add depth."
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
