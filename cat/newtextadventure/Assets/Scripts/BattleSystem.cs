using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    START, PLAYERTURN, PLAYER2TURN, ENEMYTURN, WON, LOST, PLAYERITEM
}

public class BattleSystem : MonoBehaviour
{
    public Button AttackBtn;
    public Button MagicBtn;
    public Button ItemBtn;

    public GameObject playerPrefab;
    public GameObject player2Prefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform player2BattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit player2Unit;
    Unit enemyUnit;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD player2HUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    void Start()
    {
        AttackBtn.interactable = false;
        MagicBtn.interactable = false;
        ItemBtn.interactable = false;

        state = BattleState.START;
        SetupBattle();
        StartCoroutine(SetupBattle());
    }

    IEnumerator  SetupBattle()
    {
        AttackBtn.interactable = false;

        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject player2GO = Instantiate(player2Prefab, player2BattleStation);
        player2Unit = player2GO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = enemyUnit.unitName + "Appears!";

        playerHUD.SetHUD(playerUnit);
        player2HUD.SetHUD(player2Unit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(0.1f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        AttackBtn.interactable = true;
        dialogueText.text = "Fighter:";
        ItemBtn.interactable = true;
    }

    void Player2Turn()
    {
        ItemBtn.interactable = false;
        dialogueText.text = "Black Mage:";
        MagicBtn.interactable = true;
    }

    IEnumerator PlayerItem()
    {
        AttackBtn.interactable = false;

        playerUnit.Heal(5);

        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "Healed!";

        yield return new WaitForSeconds(1f);

        state = BattleState.PLAYER2TURN;
        Player2Turn();
    }

    IEnumerator PlayerAttack()
    {
        MagicBtn.interactable = false;

        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.SetHP(enemyUnit.currentHP);

        yield return new WaitForSeconds(0.1f);

        if(isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            AttackBtn.interactable = false;
            state = BattleState.PLAYER2TURN;
            Player2Turn();
        } 
        
    }

    IEnumerator Player2Attack()
    {
        AttackBtn.interactable = false;
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.SetHP(enemyUnit.currentHP);

        yield return new WaitForSeconds(0.1f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }

    IEnumerator EnemyTurn()
    {
        MagicBtn.interactable = false;
        AttackBtn.interactable = false;

        dialogueText.text = enemyUnit.unitName + " attacks!";

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if(isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }


    void EndBattle()
    {
        if(state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
        }
    }



    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;



        StartCoroutine(PlayerAttack());
    }

    public void OnMagicButton()
    {
        if (state != BattleState.PLAYER2TURN)
            return;



        StartCoroutine(Player2Attack());
    }

    public void OnItemButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;


      StartCoroutine(PlayerItem());
    }
}
