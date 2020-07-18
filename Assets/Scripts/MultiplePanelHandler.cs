using UnityEngine;
using UnityEngine.UI;

public class MultiplePanelHandler : MonoBehaviour
{
    public int currentlyShown = 0;
    public Button left;
    public Button right;

    public void IncreaseCurrentlyShown()
    {
        if (currentlyShown + 1 < transform.childCount)
        {
            currentlyShown++;
            Redraw();
        }
    }
    public void DecreaseCurrentlyShown()
    {
        if (currentlyShown > 0)
        {
            currentlyShown--;
            Redraw();
        }
    }

    private void Redraw()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == currentlyShown)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        left.interactable = true;
        right.interactable = true;
        if (currentlyShown == 0)
        {
            left.interactable = false;
        }
        if (currentlyShown == transform.childCount - 1)
        {
            right.interactable = false;
        }
    }

    private void Start()
    {
        Redraw();
    }
}
