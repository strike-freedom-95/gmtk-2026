using System;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public int strength = 1;
    public int endurance = 1;
    public int agility = 1;
    public int accuracy = 5;
    public int aggression = 5;
    public int focus = 5;
    public int recovery = 10; 

    public string result = "";

    public static PlayerStatsManager Instance {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetStrength()
    {
        return strength;
    }

    public int GetEndurance()
    {
        return endurance;
    }

    public int GetAgility()
    {
        return agility;
    }

    public int GetAccuracy()
    {
        return accuracy;
    }

    public int GetAggression()
    {
        return aggression;
    }

    public int GetFocus()
    {
        return focus;
    }

    public int GetRecovery()
    {
        return recovery;
    }

    public void SetStrength(int value)
    {
        strength = value;
    }

    public void SetEndurance(int value)
    {
        endurance = value;
    }

    public void SetAgility(int value)
    {
        agility = value;
    }

    public void SetAccuracy(int value)
    {
        accuracy = value;
    }

    public void SetAggression(int value)
    {
        aggression = value;
    }

    public void SetFocus(int value)
    {
        focus = value;
    }

    public void SetRecovery(int value)
    {
        recovery = value;
    }

    public int[] GetAllStats()
    {
        int[] allStat = {strength, endurance, agility, accuracy, aggression, focus, recovery};

        return allStat;
    }

    public void IncreaseStrength()
    {
        strength  = Math.Clamp(strength + 3, 0, 10);
        agility = Math.Clamp(agility - 1, 0, 10);
    }

    public void IncreaseEndurance()
    {
        endurance  = Math.Clamp(endurance + 3, 0, 10);
        aggression = Math.Clamp(aggression - 1, 0, 10);
    }

    public void IncreaseSpeed()
    {
        agility  = Math.Clamp(agility + 3, 0, 10);
        strength = Math.Clamp(strength - 1, 0, 10);
    }

    public void IncreaseAccuracy()
    {
        accuracy  = Math.Clamp(accuracy + 3, 0, 10);
        aggression = Math.Clamp(aggression - 1, 0, 10);
    }

    public void IncreaseAggression()
    {
        aggression  = Math.Clamp(aggression + 3, 0, 10);
        focus = Math.Clamp(focus - 1, 0, 10);
    }

    public void IncreaseFocus()
    {
        focus  = Math.Clamp(focus + 3, 0, 10);
        agility = Math.Clamp(agility - 1, 0, 10);
    }

    public void IncreaseRecovery()
    {
        recovery  = Math.Clamp(recovery + 3, 0, 10);
        strength = Math.Clamp(strength - 1, 0, 10);
    }
}
