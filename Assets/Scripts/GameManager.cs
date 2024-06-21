using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int maxHealth = 100;
    public int health;

    public Inventory inventory;

    public bool hasDash = false;
    public bool hasWallRun = false;
    public bool hasWeaponThrow = false;

    [SerializeField] private UIInventory uiInventory;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        Time.timeScale = 1f;
        inventory = new Inventory();
        uiInventory.SetInventory(inventory);
    }

    public void Start()
    {
        health = maxHealth;

        ItemWorld.SpawnItemWorld(new Vector3(5, 0.5f, 5), new Item { itemType = Item.ItemType.Sword, amount = 1});
        ItemWorld.SpawnItemWorld(new Vector3(-5, 0.5f, 5), new Item { itemType = Item.ItemType.Key, amount = 1 });
        ItemWorld.SpawnItemWorld(new Vector3(0, 0.5f, -5), new Item { itemType = Item.ItemType.Coin, amount = 1 });
    }

    public void EnablePowerUp(int powerUp)
    {
        if (powerUp <= 1) hasDash = true;
        if (powerUp == 2) hasWallRun = true;
        if (powerUp >= 3) hasWeaponThrow = true;
    }

    public void ChangeHealth(int health)
    {
        health += health;
        if (health <= 0 ) { PlayerDeath(); }
    }
    public void PlayerDeath()
    {
        SceneManager.LoadScene("DeathScreen");
    }
}
