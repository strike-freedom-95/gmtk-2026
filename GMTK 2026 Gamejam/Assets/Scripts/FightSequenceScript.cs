using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightSequenceScript : MonoBehaviour
{
    [SerializeField] Slider playerHealth;
    [SerializeField] Slider opponentHealth;

    [SerializeField] CanvasGroup controlGroup;
    [SerializeField] CanvasGroup commentaryCanvasGrp;
    [SerializeField] TextMeshProUGUI commentaryText;

    [Header("Players")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject opponent;

    FighterStats fs;
    OpponentStats os;

    bool playerFainted;
    bool opponentFainted;
    bool isFighting;

    public enum PlayerMove
    {
        attack,
        block,
        evade,
        giveup
    }

    PlayerMove playerMove;

    void Awake()
    {
        fs = player.GetComponent<FighterStats>();
        os = opponent.GetComponent<OpponentStats>();

        playerHealth.value = fs.maxHealth;
        opponentHealth.value = os.maxHealth;

        commentaryCanvasGrp.alpha = 0;

        EnableControls();
    }


    public void AttackOption()
    {
        SelectMove(PlayerMove.attack);
    }

    public void BlockOption()
    {
        SelectMove(PlayerMove.block);
    }

    public void EvadeOption()
    {
        SelectMove(PlayerMove.evade);
    }

    public void GiveupOption()
    {
        SelectMove(PlayerMove.giveup);
    }


    void SelectMove(PlayerMove move)
    {
        if (isFighting || playerFainted || opponentFainted)
            return;

        playerMove = move;

        StartCoroutine(AttackSequence());
    }


    void EnableControls()
    {
        controlGroup.alpha = 1;
        controlGroup.interactable = true;
        controlGroup.blocksRaycasts = true;
    }


    void DisableControls()
    {
        controlGroup.alpha = 0;
        controlGroup.interactable = false;
        controlGroup.blocksRaycasts = false;
    }


    void ShowCommentary(string message)
    {
        commentaryCanvasGrp.alpha = 1;
        commentaryText.text = message;
    }

    void PlayerGiveUp()
    {
        playerFainted = true;

        DisableControls();

        ShowCommentary("You gave up!");
        StartCoroutine(GoBackToFirstSceneAfterDelay());

        Debug.Log("Player surrendered!");
    }

    IEnumerator GoBackToFirstSceneAfterDelay()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);
    }



    public void UpdatePlayerHealth(float value)
    {
        playerHealth.value += value;

        playerHealth.value = Mathf.Clamp(
            playerHealth.value,
            0,
            fs.maxHealth
        );

        CheckFaint();
    }


    public void UpdateOpponentHealth(float value)
    {
        opponentHealth.value += value;

        opponentHealth.value = Mathf.Clamp(
            opponentHealth.value,
            0,
            os.maxHealth
        );

        CheckFaint();
    }



    void CheckFaint()
    {
        if (playerHealth.value <= 0 && !playerFainted)
        {
            playerFainted = true;

            DisableControls();
            fs.PlayFaintAnimation();

            ShowCommentary(
                "Player has fainted!"
            );

            PlayerStatsManager.Instance.result = "You have Won!";

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }


        if (opponentHealth.value <= 0 && !opponentFainted)
        {
            opponentFainted = true;

            DisableControls();
            os.PlayFaintAnimation();

            ShowCommentary(
                "Opponent has fainted!"
            );

            PlayerStatsManager.Instance.result = "You have Lost!";

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }



    bool GetCritical(int focus)
    {
        return Random.value <
               focus * 0.05f;
    }


    float CalculateDamage(
        float strength,
        int focus
    )
    {
        float damage = strength;


        if (GetCritical(focus))
        {
            damage *= 1.5f;

            ShowCommentary(
                "Critical Hit!"
            );
        }


        return damage;
    }



    void ApplyRecovery(bool isPlayer)
    {
        if (playerFainted || opponentFainted)
            return;


        float currentHP =
            isPlayer ?
            playerHealth.value :
            opponentHealth.value;


        float maxHP =
            isPlayer ?
            fs.maxHealth :
            os.maxHealth;


        if (currentHP > maxHP * 0.5f)
            return;


        int recovery =
            isPlayer ?
            fs.GetRecovery() :
            os.GetRecovery();


        if (Random.value < recovery * 0.05f)
        {
            if (isPlayer)
                UpdatePlayerHealth(3);
            else
                UpdateOpponentHealth(3);


            ShowCommentary(
                "Recovered!"
            );
        }
    }


    void ResolveTurn(
    PlayerMove playerAction,
    OpponentStats.Move enemyAction
)
    {
        if (playerAction == PlayerMove.attack)
        {
            ResolvePlayerAttack(enemyAction);
        }


        else if (playerAction == PlayerMove.block)
        {

            ResolvePlayerBlock(enemyAction);
        }


        else if (playerAction == PlayerMove.evade)
        {
            ResolvePlayerEvade(enemyAction);
        }
    }

    float CalculateDamageTaken(float damage, int endurance)
    {
        // Every point of endurance gives 5% damage reduction
        float reduction = endurance * 0.05f;

        // Cap damage reduction at 50%
        reduction = Mathf.Clamp(
            reduction,
            0f,
            0.5f
        );

        float finalDamage = damage * (1 - reduction);

        return finalDamage;
    }


    void ResolveOpponentEvade()
    {
        Debug.Log("Opponent Evade");


        float evadeChance =
            0.4f +
            (os.GetAgility() * 0.05f) -
            (fs.GetAccuracy() * 0.05f);


        evadeChance = Mathf.Clamp(
            evadeChance,
            0.1f,
            0.9f
        );


        if (Random.value < evadeChance)
        {
            ShowCommentary(
                "Opponent dodged!"
            );


            // Perfect evade counter chance
            if (Random.value < os.GetFocus() * 0.05f)
            {
                ShowCommentary(
                    "Perfect Evade Counter!"
                );


                float counterDamage =
                    os.GetStrength() * 0.5f;


                counterDamage = CalculateDamageTaken(
                    counterDamage,
                    fs.GetEndurance()
                );


                UpdatePlayerHealth(
                    -counterDamage
                );
            }
        }
        else
        {
            float damage =
                CalculateDamage(
                    fs.GetStrength(),
                    fs.GetFocus()
                );


            damage = CalculateDamageTaken(
                damage,
                os.GetEndurance()
            );


            UpdateOpponentHealth(
                -damage
            );


            ShowCommentary(
                "Attack landed!"
            );
        }
    }


    void ResolvePlayerAttack(
    OpponentStats.Move enemyMove
)
    {
        fs.PlayAttackAnimation();
        if (enemyMove == OpponentStats.Move.block)
        {
            os.PlayOpponentBlock();
            float damage =
                fs.GetStrength() * 0.5f;


            bool perfectBlock =
                Random.value <
                os.GetFocus() * 0.05f;


            if (perfectBlock)
            {
                damage = 0;

                ShowCommentary(
                    "Opponent Perfect Block!"
                );
            }


            UpdateOpponentHealth(-damage);
            return;
        }


        if (enemyMove == OpponentStats.Move.evade)
        {
            ResolveOpponentEvade();
            return;
        }


        // Attack vs Attack

        if (fs.GetAgility() >= os.GetAgility())
        {
            PlayerAttack();

            if (!opponentFainted)
                EnemyAttack();
        }
        else
        {
            EnemyAttack();

            if (!playerFainted)
                PlayerAttack();
        }
    }

    void ResolvePlayerBlock(
        OpponentStats.Move enemyMove
    )
    {
        fs.PlayBlockAnimation();
        if (enemyMove != OpponentStats.Move.attack)
        {
            ShowCommentary(
                "Nothing happened."
            );

            return;
        }


        float damage =
            os.GetStrength() * 0.5f;


        bool perfectBlock =
            Random.value <
            fs.GetFocus() * 0.05f;


        if (perfectBlock)
        {
            damage = 0;

            ShowCommentary(
                "Perfect Block!"
            );


            UpdateOpponentHealth(
                -(fs.GetStrength() * 0.3f)
            );
        }


        UpdatePlayerHealth(-damage);
    }

    void ResolvePlayerEvade(
        OpponentStats.Move enemyMove
    )
    {
        if (enemyMove != OpponentStats.Move.attack)
        {
            ShowCommentary(
                "Nothing happened."
            );

            return;
        }


        float evadeChance =
            0.4f +
            (fs.GetAgility() * 0.05f) -
            (os.GetAccuracy() * 0.05f);


        evadeChance = Mathf.Clamp(
            evadeChance,
            0.1f,
            0.9f
        );


        if (Random.value < evadeChance)
        {
            ShowCommentary(
                "Player dodged!"
            );


            if (Random.value <
               fs.GetFocus() * 0.05f)
            {
                ShowCommentary(
                    "Perfect Evade!"
                );


                UpdateOpponentHealth(
                    -(fs.GetStrength() * 0.5f)
                );
            }
        }
        else
        {
            EnemyAttack();
        }
    }

    IEnumerator AttackSequence()
    {
        isFighting = true;

        DisableControls();


        OpponentStats.Move enemyMove =
            os.GetRandomMove();


        yield return new WaitForSeconds(1);


        if (playerMove == PlayerMove.giveup)
        {
            PlayerGiveUp();

            isFighting = false;
            yield break;
        }


        ResolveTurn(
            playerMove,
            enemyMove
        );


        yield return new WaitForSeconds(1);


        if (!playerFainted && !opponentFainted)
        {
            ApplyRecovery(true);
            ApplyRecovery(false);
        }


        yield return new WaitForSeconds(0.5f);


        if (!playerFainted && !opponentFainted)
        {
            EnableControls();
        }


        commentaryCanvasGrp.alpha = 0;


        isFighting = false;
    }


    void ResolveAttack()
    {
        Debug.Log("ATTACK");


        bool playerFirst = fs.GetAgility() >= os.GetAgility();


        if (playerFirst)
        {
            PlayerAttack();
        }
        else
        {
            EnemyAttack();
        }
    }

    void PlayerAttack()
    {
        if (Random.value <= fs.GetAccuracy() * 0.1f)
        {
            float damage =
                CalculateDamage(
                    fs.GetStrength(),
                    fs.GetFocus()
                );

            UpdateOpponentHealth(-damage);

            ShowCommentary(
                "Player attack!"
            );
        }
        else
        {
            ShowCommentary(
                "Player missed!"
            );
        }
    }



    void EnemyAttack()
    {
        os.PlayOpponentAttack();
        if (Random.value <= os.GetAccuracy() * 0.1f)
        {
            float damage =
                CalculateDamage(
                    os.GetStrength(),
                    os.GetFocus()
                );


            UpdatePlayerHealth(-damage);

            ShowCommentary(
                "Opponent attack!"
            );
        }
        else
        {
            ShowCommentary(
                "Opponent missed!"
            );
        }
    }


    /*void EnemyAttack()
    {
        if (Random.Range(1, 11) <= os.GetAccuracy())
        {
            float damage = CalculateDamage(
                os.GetStrength(),
                os.GetFocus()
            );

            UpdatePlayerHealth(-damage);

            ShowCommentary("Enemy attack!");
        }
        else
        {
            ShowCommentary("Enemy missed!");
        }
    }*/

    void ResolveBlock()
    {
        Debug.Log("BLOCK");


        if (Random.Range(1, 11) <= fs.GetAccuracy())
        {
            float damage = fs.GetStrength() * 0.5f;


            bool perfectBlock =
                Random.value < os.GetFocus() * 0.05f;


            if (perfectBlock)
            {
                damage = 0;

                ShowCommentary("Perfect Block!");

                UpdatePlayerHealth(
                    -(os.GetStrength() * 0.3f)
                );
            }


            UpdateOpponentHealth(-damage);
        }
        else
        {
            ShowCommentary("Missed!");
        }
    }

    void ResolveEvade()
    {
        Debug.Log("EVADE");


        float evadeChance =
            0.4f +
            (os.GetAgility() * 0.05f) -
            (fs.GetAccuracy() * 0.05f);


        evadeChance = Mathf.Clamp(
            evadeChance,
            0.1f,
            0.9f
        );


        if (Random.value < evadeChance)
        {
            ShowCommentary("Opponent evaded!");


            if (Random.value < os.GetFocus() * 0.05f)
            {
                ShowCommentary("Perfect Evade!");

                UpdatePlayerHealth(
                    -(os.GetStrength() * 0.5f)
                );
            }
        }
        else
        {
            float damage = CalculateDamage(
                fs.GetStrength(),
                fs.GetFocus()
            );

            UpdateOpponentHealth(-damage);

            ShowCommentary("Attack landed!");
        }
    }
}
