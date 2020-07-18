using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Handles whatever is displayed on the UI and also acts as a Game Manager and tower spawner
public class GameManager : MonoBehaviour
{
    //Allows you to reference GameManager in a static manner. This also means that there cannot be more than
    //one GameManager script in the scene at once.
    public static GameManager SharedInstance;

    //Global variables for this game
    [SerializeField]
    private int baseHealth, startingCash;
    [SerializeField]
    private GameObject exitPoint;
    private int currentHealth, cash, score, numberOfTowers;
    private bool hasLost;
    private float costMultiplier;
    [SerializeField]
    private float baseCostMultiplier;

    //UI Elements for this game
    public Text healthIndicator, cashIndicator, difficultyIndicator, finalScore;
    public GameObject pauseMenu, gameOverMenu;
    public Button basicButton, burstButton, slowButton, laserButton, chainButton, sellButton;

    //Holds the tower to be placed, or if the tower is to be sold
    //This will be accessed by the tower spawning script
    public string CurrentSelection;
    private GameObject selectedTower;
    [SerializeField]
    private GameObject basicTower, burstTower, slowTower, laserTower, chainTower;

    public void SetCurrentSelection(string currentSelection)
    {
        CurrentSelection = currentSelection;
        UpdateUIElements();
    }

    //These are game-specific variables. Feel free to ignore
    [SerializeField]
    private float difficultyRamp, difficultyLevel,
        smallEnemyHealthScale, bigEnemyHealthScale,
        bigEnemyHealthScale2, bossHealthScale, bossHealthScale2;

    //Exposes private variables but make them read-only
    public float DifficultyLevel { get { return difficultyLevel; } }
    public float NumberOfTowers { get { return numberOfTowers; } }
    public float[] EnemyHealthScales
    {
        get
        {
            return new float[] {
                smallEnemyHealthScale,
                bigEnemyHealthScale,
                bigEnemyHealthScale2,
                bossHealthScale,
                bossHealthScale2
            };
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateUIElements();
    }

    public void ConvertEnemyToCash(GameObject enemy)
    {
        //Extracts the base cash variable from the enemy's Enemy Attributes script
        int baseCash = enemy.GetComponent<EnemyAttributes>().BaseCash;
        //Takes the product of difficulty level and base cash, and converts it to an integer by rounding down
        cash += (int)(difficultyLevel * baseCash);

        //In this game, score is the total accumulated cash
        score += (int)(difficultyLevel * baseCash);
        UpdateUIElements();
    }

    public void Pause()
    {
        //Is the game already paused?
        if (Time.timeScale == 0f)
        {
            //Resume
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
        else
        {
            //If the game isn't already paused, pause.
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
    }

    public void Restart()
    {
        HealthbarSpawn.instance.DeallocateHealthbar();
        ObjectPooler.SharedInstance.DisableAllObjects();
        //Reloads the current scene, and turns off the pause menu and game over menu just in case
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        UpdateUIElements();
    }

    public void ReturnToMenu()
    {
        HealthbarSpawn.instance.DeallocateHealthbar();
        ObjectPooler.SharedInstance.DisableAllObjects();
        SceneManager.LoadScene("Menu");
    }

    //Game-specific function that updates UI elements.
    private void UpdateUIElements()
    {
        //Update texts
        healthIndicator.text = currentHealth.ToString();
        cashIndicator.text = cash.ToString();

        //Checks the tower that is selected, and disables the button accordingly. For example, if "Basic Tower"
        //is selected, then the corresponding button is disabled so that you can't click it again

        //Initialises the checking process by setting all to true, then checks all the remaining cases
        basicButton.interactable = true;
        burstButton.interactable = true;
        slowButton.interactable = true;
        laserButton.interactable = true;
        chainButton.interactable = true;
        sellButton.interactable = true;
        switch (CurrentSelection)
        {
            case "Basic":
                basicButton.interactable = false;
                break;
            case "Burst":
                burstButton.interactable = false;
                break;
            case "Slow":
                slowButton.interactable = false;
                break;
            case "Laser":
                laserButton.interactable = false;
                break;
            case "Chain":
                chainButton.interactable = false;
                break;
            case "Sell":
                sellButton.interactable = false;
                break;
            default:
                break;
        }
        //As the game gets larger with more towers being implemented, you may use a dictionary instead to eliminate
        //the need for switch statements.

        //For each of the buttons, reflect the cost
        List<Button> listOfButtons = new List<Button>() { basicButton, burstButton, slowButton, laserButton, chainButton };
        foreach (Button button in listOfButtons)
        {
            //For each button, finds a child object named "Cost" and gets the text component of that object
            Text buttonText = button.transform.Find("Cost").GetComponent<Text>();

            //Finds the corresponding tower related to the button
            GameObject correspondingTower = null;
            if (button == basicButton) correspondingTower = basicTower;
            else if (button == burstButton) correspondingTower = burstTower;
            else if (button == slowButton) correspondingTower = slowTower;
            else if (button == laserButton) correspondingTower = laserTower;
            else if (button == chainButton) correspondingTower = chainTower;

            //On hindsight, I should have put the towers and the buttons into lists, and reference their 
            //positions instead.
            if (CurrentSelection == "Sell")
            {
                buttonText.text = correspondingTower.
                    GetComponent<TowerAttributes>().BaseCost.ToString();
                buttonText.CrossFadeColor(Color.cyan, 0.2f, true, true);
            }
            else
            {
                int cost = CalculateCost(correspondingTower);
                buttonText.text = cost.ToString();
                if (cash >= cost)
                {
                    buttonText.CrossFadeColor(new Color(0f, 1f, 0.14f), 0.2f, true, true);
                }
                else
                {
                    buttonText.CrossFadeColor(new Color(1f, 0.8f, 0f), 0.2f, true, true);
                }
            }
        }
    }

    //This function accesses a script within exitPoint to return a list of enemies colliding with exitPoint
    private List<GameObject> GetEnemiesInRegion(GameObject gameObject)
    {
        return gameObject.GetComponent<ExitPoint>().EnemyList;
    }

    //This portion handles tower spawning
    private Ray ray;
    private RaycastHit hit;
    private int rayCastMask;

    private int GetRaycastMask()
    {
        //The mouse is supposed to be blocked only by the Enemy Layer, the Floor Layer, UI and the Tower Layer
        //Previously, particle effects blocked the mouse, preventing buying and selling of towers
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int floorLayer = LayerMask.NameToLayer("Floor");
        int towerLayer = LayerMask.NameToLayer("Tower");
        int UILayer = LayerMask.NameToLayer("UI");
        int defaultLayer = LayerMask.NameToLayer("Default");

        //If the enemy layer is under layer 6, this returns a binary number 00100000 because the 1 is shifted
        //left by 6 bits
        var layerMask1 = 1 << enemyLayer;
        var layerMask2 = 1 << floorLayer;
        var layerMask3 = 1 << towerLayer;
        var layerMask4 = 1 << UILayer;
        var layerMask5 = 1 << defaultLayer;

        //If we have three layers on layers 2, 3 and 6, finalMask would be equal to 00100110. Now we just need to pass
        //this through the raycaster, and the mouse will only detect objects in the floor layer, the enemy layer, and
        //the tower layer, and nothing else. 
        var finalMask = layerMask1 | layerMask2 | layerMask3 | layerMask4 | layerMask5;
        return finalMask;
    }

    private bool MouseIsOverFloor()
    {
        //Draws a ray and check if the mouse is over an enemy, floor or tower
        if (Physics.Raycast(ray: ray, hitInfo: out hit, maxDistance: 100f, layerMask: rayCastMask))
        {
            //If the mouse is over the floor, the function returns true. Also, Physics.Raycast(ray, out hit))
            //implicitly modifies the value of hit due to the "out" keyword
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
            {
                return true;
            }
        }
        return false;
    }

    private bool MouseIsOverTower()
    {
        //Draws a ray and check if the mouse is over an enemy, floor or tower
        if (Physics.Raycast(ray: ray, hitInfo: out hit, maxDistance: 100f, layerMask: rayCastMask))
        {
            //If the mouse is over the floor, the function returns true. Also, Physics.Raycast(ray, out hit))
            //implicitly modifies the value of hit due to the "out" keyword
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Tower"))
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnTower(GameObject tower, Vector3 location)
    {
        GameObject towerInstance = ObjectPooler.SharedInstance.GetPooledObject(tower.tag);
        towerInstance.transform.position = location;
        towerInstance.transform.rotation = Quaternion.identity;

        towerInstance.SetActive(true);
        numberOfTowers += 1;
        UpdateUIElements();
    }

    private void SellTower(GameObject tower)
    {
        //Extracts the base cost of the tower, and refunds it. After which, disables the tower and 
        //reflect the changes on the number of towers accordingly.
        int refundValue = tower.GetComponent<TowerAttributes>().BaseCost;
        cash += refundValue;
        tower.SetActive(false);
        numberOfTowers -= 1;
        UpdateUIElements();
    }

    private int CalculateCost(GameObject tower)
    {
        //Takes base cost multiplied by the cost multiplier, and returns it as an integer by rounding it down.
        int baseCost = tower.GetComponent<TowerAttributes>().BaseCost;
        costMultiplier = Mathf.Pow(baseCostMultiplier, numberOfTowers);
        return (int)(baseCost * costMultiplier);
    }

    private void Awake()
    {
        SharedInstance = this;
        rayCastMask = GetRaycastMask();
    }

    // Use this for initialization
    private void OnEnable()
    {
        currentHealth = baseHealth;
        cash = startingCash;
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        CurrentSelection = "Basic";
        hasLost = false;
        numberOfTowers = 0;
        Time.timeScale = 1f;
        score = 0;
    }

    private void Start()
    {
        UpdateUIElements();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.timeScale > 0f)
        {
            difficultyIndicator.text = "Difficulty\n" + DifficultyLevel.ToString("0.000");
        }

        //This portion handles what happens if the enemy reaches the opposite side of the screen
        List<GameObject> enemyList = GetEnemiesInRegion(exitPoint);

        //For each enemy, check their attack strength and deduct health accordingly before disabling the enemy
        if (enemyList.Count > 0)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                GameObject currentEnemy = enemyList[i];
                int damage = currentEnemy.GetComponent<EnemyAttributes>().AttackStrength;
                TakeDamage(damage);
                currentEnemy.SetActive(false);
            }
        }

        //This part handles the Game Over menu
        if (currentHealth <= 0f)
        {
            hasLost = true;
        }

        if (hasLost)
        {
            //If the game over menu isn't already active, make it active, and pause the game
            if (gameOverMenu.activeSelf == false)
            {
                //Updates the final score before showing the game over screen
                finalScore.text = "Final score " + score.ToString();
                gameOverMenu.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        //This part handles pausing and restarting
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }

        //Takes what's in the field CurrentSelection and finds the appropriate tower and puts it in selectedTower
        switch (CurrentSelection)
        {
            case "Basic":
                selectedTower = basicTower;
                break;
            case "Burst":
                selectedTower = burstTower;
                break;
            case "Slow":
                selectedTower = slowTower;
                break;
            case "Laser":
                selectedTower = laserTower;
                break;
            case "Chain":
                selectedTower = chainTower;
                break;
            default:
                selectedTower = basicTower; //Failsafe
                break;
        }

        //This part handles tower placement
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Input.GetMouseButtonDown(0) means click, whereas Time.timeScale > 0f means the game isn't paused
        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0f)
        {
            if (CurrentSelection == "Sell")
            {
                if (MouseIsOverTower())
                {
                    //In this game, the part of the tower clicked is not the tower itself, but the mesh
                    GameObject clickedTowerMesh = hit.transform.gameObject;
                    //Hence, we need to reference the parent instead
                    GameObject clickedTower = clickedTowerMesh.transform.parent.gameObject;
                    SellTower(clickedTower);
                }
            }
            else
            {
                if (MouseIsOverFloor())
                {
                    //Do we have sufficient cash to afford this tower?
                    if (cash >= CalculateCost(selectedTower))
                    {
                        cash -= CalculateCost(selectedTower);

                        //Note that MouseIsOverFloor() implicitly modifies the value of hit. As such,
                        //we can get the hit location by using hit.point
                        SpawnTower(selectedTower, hit.point);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        difficultyLevel += difficultyRamp * Time.fixedDeltaTime;
    }
}
