using UnityEngine;

public class SoleTowerBuff : MonoBehaviour
{
    public int jumpIncreases = 0;
    public float jumpRangeIncrease = 0;
    public float splashRangeIncrease = 0;
    public float cooldownIncrease = 0;
    public float damageIncrease = 0;
    public float rangeIncrease = 0;

    [SerializeField]
    private GameObject soleTowerBuff;

    private void Update()
    {
        if (GameManager.SharedInstance.NumberOfTowers == 1)
        {
            if (!soleTowerBuff.activeSelf)
                soleTowerBuff.SetActive(true);
        } else
        {
            if (soleTowerBuff.activeSelf)
                soleTowerBuff.SetActive(false);
        }
    }
}
