using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _items;
    
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _camTransform;
    [SerializeField] private UIManager _UIManager;
    [SerializeField] private float timeAllowed = 90f;

    // Loot grammar rules
    private List<string> adjectives = new List<string> { "Crusty", "Heavy", "Based", "Cursed", "Shiny"};
    private List<string> materials = new List<string> { "Iron", "Gold", "Lava", "Crystal", "Cyber"};
    private List<string> types = new List<string> { "Rocks", "Beeds", "Tokens", "Jordans", "Coins" };

    private float timeRemaining;
    private int playerScore;
    private Vector3 startPosition = new Vector3(563.599976f, 11.96f, 438.799988f);
    private Vector3 camPosition = new Vector3(575.400024f, 28.7999992f, 409f);
    private Quaternion camRotation = new Quaternion(0.0591459349f, -0.307017475f, 0.0120778931f, 0.949787438f);
    private float gameOverDelay = 3f;

    private void Start()
    {
        // seed random number generator
        Random.InitState(System.DateTime.Now.Millisecond);
        _UIManager.UpdateTimerCount((int)timeAllowed);

        UserSettings.IsGameOver = true;
        UserSettings.IsPaused = false;
    }

    public void StartGame()
    {
        Debug.Log("Starting Game");
        UserSettings.IsGameOver = false;
        UserSettings.IsPaused = false;
        ActivateRandom(); //Activate first star
        StartCoroutine(nameof(StartTimer)); // start timer
    }

    public void EndGame()
    {
        // deactivate all remaining stars
        for (int i = 0; i < _items.transform.childCount; i++) { _items.transform.GetChild(i).gameObject.SetActive(false); }

        // player respawn
        _playerTransform.position = startPosition;
        _playerTransform.rotation = Quaternion.identity;

        // camera respawn
        _camTransform.position = camPosition;
        _camTransform.rotation = camRotation;

        // reset time and score
        playerScore = 0;
        _UIManager.UpdateScoreCount(playerScore);
        _UIManager.UpdateTimerCount((int)timeAllowed);

        //reset game states
        UserSettings.IsGameOver = true;
    }

    // called whenever player collects a star
    public void StarCollected()
    {
        // generate random loot
        (string, int) randomLoot = GenerateLootDescription();
        // display loot and value
        _UIManager.UpdateLootText(randomLoot.Item1 + " Found!");
        _UIManager.UpdateIncrement(randomLoot.Item2);

        // fade out loot text
        _UIManager.StartFadeOut();

        // update score
        playerScore += randomLoot.Item2;
        _UIManager.UpdateScoreCount(playerScore);
        
        ActivateRandom();
    }

    // activate random star with random primary and secondary colors, and random bonus points
    private void ActivateRandom()
    {
        Debug.Log("Activating Random Star");
        if (_items.transform.childCount > 0)
        {
            int randomIndex = Random.Range(0, _items.transform.childCount);
            Transform selectedChild = _items.transform.GetChild(randomIndex);
            selectedChild.gameObject.SetActive(true);

            // iterate through primary and secondary color groups and randomize color
            Transform selectedStar = _items.transform.GetChild(randomIndex);
            foreach (Transform cubeGroup in selectedStar) RandomizeColor(cubeGroup);
        }
    }

    // Takes a group of cubes and randomizes the material color
    private void RandomizeColor(Transform group)
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value);

        foreach (Transform cube in group)
        {
            Material cubeMaterial = cube.GetComponent<Renderer>().material;
            cubeMaterial.color = randomColor;
        }
    }

    // Coroutine: Starts a timer and when time runs out, ends the game, and plays again after a delay
    private IEnumerator StartTimer()
    {
        timeRemaining = timeAllowed;
        
        while (timeRemaining > 0f)
        {
            // timer does not decreememnt if is paused
            if (!UserSettings.IsPaused)
            {
                _UIManager.UpdateTimerCount((int)timeRemaining);
                timeRemaining -= Time.deltaTime;
            }
            yield return null;
        }

        _UIManager.UpdateTimerCount(0);

        UserSettings.IsGameOver = true; // keep this here so player movment stops at the end of the timer

        // wait a few secs before ending the game
        Invoke(nameof(EndGame), gameOverDelay);
        Invoke(nameof(PlayAgain), gameOverDelay + 0.1f);
    }
    private void PlayAgain() => _UIManager.PlayAgain(); //encapsulate instance func to call after delay


    // returns string of random loot description
    private (string, int) GenerateLootDescription()
    {
        int adjective = Random.Range(0, adjectives.Count);
        int material = Random.Range(0, materials.Count);
        int type = Random.Range(0, types.Count);

        int lootValue = 3 + adjective + material + type;
        // construct the loot description
        string lootDescription = $"{adjectives[adjective]} {materials[material]} {types[type]}";

        return (lootDescription, lootValue) ;
    }
}
