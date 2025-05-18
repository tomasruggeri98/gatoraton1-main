// MenuController.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private int selectedWidth = 8;
    private int selectedHeight = 8;

    public void Select8x8() { selectedWidth = 20; selectedHeight = 20; }
    public void Select16x8() { selectedWidth = 45; selectedHeight = 45; }
    public void Select16x16() { selectedWidth = 60; selectedHeight = 60; }

    public void StartGame()
    {
        GameSettings.GridWidth = selectedWidth;
        GameSettings.GridHeight = selectedHeight;
        SceneManager.LoadScene("GameScene");
    }
}
