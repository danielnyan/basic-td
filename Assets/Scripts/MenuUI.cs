using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour {
    public GameObject mainScreen;
    public GameObject instructionScreen;
    public GameObject aboutScreen;

	public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowMain()
    {
        mainScreen.SetActive(true);
        instructionScreen.SetActive(false);
        aboutScreen.SetActive(false);
    }

    public void ShowInstruction()
    {
        mainScreen.SetActive(false);
        instructionScreen.SetActive(true);
        aboutScreen.SetActive(false);
    }

    public void ShowAbout()
    {
        mainScreen.SetActive(false);
        instructionScreen.SetActive(false);
        aboutScreen.SetActive(true);
    }
}
