using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isGameOver = false;

    void Start()
    {
        Time.timeScale = 1.0f;
    }

    void Update()
    {

    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1.0f;
    }
}