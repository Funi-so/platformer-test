using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;

    public bool hasDash = false;
    public bool hasWallRun = false;
    public bool hasWeaponThrow = false;

    public void Start()
    {
        health = maxHealth;
    }

    public void EnablePowerUp(int power)
    {
        if(power <= 1) hasDash = true;
        if(power == 2) hasWallRun = true;
        if (power >= 3) hasWeaponThrow = true;
    }
}
