using UnityEngine;

public class FighterStats : MonoBehaviour
{
    public int strength = 1;
    public int endurance = 1;
    public int agility = 1;
    public int accuracy = 5;
    public int aggression = 5;
    public int focus = 5;
    public int recovery = 10;
    public float maxHealth = 100;

    Animator animator;
    PlayerStatsManager psm;

    void Start()
    {
        psm = PlayerStatsManager.Instance;

        if (psm == null)
        {
            strength = (int)Random.Range(1, 10);
            endurance = (int)Random.Range(1, 10);
            agility = (int)Random.Range(1, 10);
            accuracy = (int)Random.Range(1, 10);
            aggression = (int)Random.Range(1, 10);
            focus = (int)Random.Range(1, 10);
            recovery = (int)Random.Range(5, 10);
        }
        else
        {
            strength = psm.GetStrength();
            endurance = psm.GetEndurance();
            agility = psm.GetAgility();
            accuracy = psm.GetAccuracy();
            aggression = psm.GetAggression();
            focus = psm.GetFocus();
            recovery = psm.GetRecovery();
        }

        animator = GetComponent<Animator>();
    }

    public enum Move
    {
        attack, block, evade
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

    public Move GetRandomMove()
    {
        return (Move)Random.Range(0, System.Enum.GetValues(typeof(Move)).Length);
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger("isAttack");
    }

    public void PlayBlockAnimation()
    {
        animator.SetTrigger("isBlock");
    }

    public void PlayFaintAnimation()
    {
        animator.SetTrigger("isFaint");
    }
}
