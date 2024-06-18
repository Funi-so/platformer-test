using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerStats player;
    void Start()
    {
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
