using UnityEngine;

public class SetupLightning : MonoBehaviour
{
    [SerializeField]
    private GameObject start;
    [SerializeField]
    private GameObject end;

    public void Setup(Vector3 start, Vector3 end)
    {
        this.start.transform.position = start;
        this.end.transform.position = end;
    }
}
