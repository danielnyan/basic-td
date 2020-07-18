using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Buff
{
    public string name;
    public float duration;
    public float speedMultiplier;
    public bool canRefresh;
    public bool canStack;

    public Buff(string name, float duration = 0f, float speedMultiplier = 1f, bool canRefresh = true, bool canStack = false)
    {
        this.name = name;
        this.duration = duration;
        this.speedMultiplier = speedMultiplier;
        this.canRefresh = canRefresh;
        this.canStack = canStack;
    }
}

public class EnemyAttributes : MonoBehaviour
{
    // Contains the generic attributes such as health, as well as basic methods like applying damage
    [SerializeField]
    private float baseSpeed;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float baseHealth;
    private float currentHealth;
    [SerializeField]
    private int attackStrength;
    [SerializeField]
    private GameObject deathAnimation;
    [SerializeField]
    private int baseCash; //Cash awarded when the enemy is destroyed, which scales with difficulty level
    [SerializeField]
    private string enemyType;

    // Debuff system
    private List<Buff> buffList;
    private void BuffHandler()
    {
        speed = baseSpeed;

        //Checks whether each buff has timed out. If so, remove the buff
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].duration < 0f)
            {
                buffList.RemoveAt(i);
            }
        }

        //Iterates through each buff and adjusts the parameters accordingly. Also reduces the duration of the buff
        foreach (Buff buff in buffList)
        {
            speed *= buff.speedMultiplier;
            buff.duration -= Time.fixedDeltaTime;
        }
    }

    private void ApplyBuff(Buff buff)
    {
        //If the buff stacks, add the buff to the buff list
        if (buff.canStack)
        {
            buffList.Add(buff);
        }
        else
        {
            bool buffExists = false;

            //For each buff in buffList. For example, buffList[0] is the first buff (counting starts from zero)
            for (int i = 0; i < buffList.Count; i++)
            {
                //If the name of the buff to be applied matches the names of any of the currently applied buff...
                if (buff.name == buffList[i].name)
                {
                    //If the buff is refreshable, remove the old buff to make way for the new one
                    if (buff.canRefresh)
                    {
                        buffList.RemoveAt(i);
                    }
                    else
                    {
                        //The buff cannot stack or be refreshed, and there is already a buff in the buff list
                        buffExists = true;
                    }
                }
            }
            if (buffExists == false)
            {
                buffList.Add(buff);
            }
        }
    }

    public void TakeBullet(GameObject bullet, bool hasBuffs = false)
    {
        //Sometimes the bullet will be in a form of a trigger, and so it will not be detected by the enemy's collider
        //As such, this method takes in a bullet, and extracts the relevant details from the bullet.
        //Do note that bullets with special effects must call this method upon entering the enemy.

        float bulletDamage = bullet.GetComponent<Projectile>().Damage;
        TakeDamage(bulletDamage);

        if (hasBuffs)
        {
            List<Buff> bulletBuffs = bullet.GetComponent<Projectile>().BulletBuffs;
            foreach (Buff buff in bulletBuffs)
            {
                ApplyBuff(buff);
            }
        }
    }

    // Setup the enemy's attributes by getting the EnemyAttributes component and calling the SetupAttributes method.
    // Example: GetComponent<EnemyAttributes>().SetupAttributes(20f, 100f, 5f)
    public void SetupAttributes(float baseSpeed, float baseHealth, int attackStrength, int baseCash)
    {
        this.baseSpeed = baseSpeed;
        this.baseHealth = baseHealth;
        this.attackStrength = attackStrength;
        this.baseCash = baseCash;

        //There is a bug where the enemy is enabled before the attributes are set up. This would help fix it
        //by setting the health to full when the enemy is set up
        currentHealth = baseHealth;
    }

    // Makes other scripts able to read the attributes in this script, but unable to modify it
    // For example, GetComponent<EnemyAttributes>().Speed returns the value of speed.
    public float Speed { get { return speed; } }
    public float BaseHealth { get { return baseHealth; } }
    public float CurrentHealth { get { return currentHealth; } }
    public int AttackStrength { get { return attackStrength; } }
    public int BaseCash { get { return baseCash; } }

    // If the enemy is hit, this would reduce the enemy's health. See OnCollisionEnter
    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    private void InstantiateHitEffect(GameObject effect, Vector3 position, Quaternion rotation)
    {
        if (effect != null)
        {
            //Gets a copy of the hit effect from the object pool
            GameObject effectInstance = ObjectPooler.SharedInstance.GetPooledObject(effect.tag);

            //Moves the hit effect to a given position and rotation
            effectInstance.transform.position = position;
            effectInstance.transform.rotation = rotation;
            effectInstance.SetActive(true);

            //The particle effect will be killed in another script called KillParticles
        }
    }

    // When the enemy spawns, the current health is set to the base health
    private void OnEnable()
    {
        float difficultyLevel = GameManager.SharedInstance.DifficultyLevel;

        // Small enemy health scale, big enemy health scale (before second boss), 
        // big enemy health scale (after second boss), boss health scale (before and after)
        // Default values: 1.2, 2, 3, 2, 3
        float[] enemyHealthScale = GameManager.SharedInstance.EnemyHealthScales;

        switch (enemyType)
        {
            case "A":
                baseHealth = Mathf.Floor(2f * Mathf.Pow(difficultyLevel, enemyHealthScale[0]));
                if (difficultyLevel > 3f) baseSpeed = 4.2f;
                else if (difficultyLevel > 2f) baseSpeed = 3.6f;
                else baseSpeed = 3f;
                attackStrength = 10;
                baseCash = 2;
                break;
            case "B":
                baseHealth = difficultyLevel > 4f ? Mathf.Floor(9f * Mathf.Pow(difficultyLevel, enemyHealthScale[2])) : Mathf.Floor(9f * Mathf.Pow(difficultyLevel, enemyHealthScale[1]));
                baseSpeed = 1f;
                attackStrength = 30;
                baseCash = 9;
                break;
            case "C":
                baseHealth = Mathf.Floor(2f * Mathf.Pow(difficultyLevel, enemyHealthScale[0]));
                baseSpeed = 2f;
                attackStrength = 10;
                baseCash = 2;
                break;
            case "D":
                baseHealth = difficultyLevel > 4f ? Mathf.Floor(40f * Mathf.Pow(difficultyLevel, enemyHealthScale[4])) : Mathf.Floor(40f * Mathf.Pow(difficultyLevel, enemyHealthScale[3]));
                baseSpeed = 0.5f;
                attackStrength = 100;
                baseCash = 40;
                break;
            default:
                break;
        }
        SetupAttributes(baseSpeed, baseHealth, attackStrength, baseCash);

        currentHealth = baseHealth;
        buffList = new List<Buff>();
    }

    private void Update()
    {
        //If the enemy's health is zero, disable the enemy's game object to signify that the enemy has died.
        if (currentHealth <= 0f)
        {
            // Here, the "hit effect" is actually the death animation. The effect is placed where the 
            // enemy is, at default rotation
            InstantiateHitEffect(deathAnimation, transform.position, Quaternion.identity);
            GameManager.SharedInstance.ConvertEnemyToCash(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        //Reduces the duration of each buff, remove expired buffs, and adjust any attributes of the enemy
        BuffHandler();
    }
}
