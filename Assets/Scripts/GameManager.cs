using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerStats player;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        Time.timeScale = 1f;
    }


    public void ChangeHealth(int health)
    {
        player.health += health;
        if (player.health <= 0 ) { PlayerDeath(); }
    }
    public void PlayerDeath()
    {
        SceneManager.LoadScene("DeathScreen");
    }
}
