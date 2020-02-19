using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cards
{
    public abstract class CardData : ScriptableObject
    {
        public CardData() {}
        public abstract CardType GetCardType();
        private CardType         cardType;


        public string           cardName => name;
        public string           cardDescription;
        public Sprite           cardArtwork;


        [Space]
        public CardStat[]       cardStats;
        [Space]
        public CardAction[]     cardActions;
    }


    [System.Serializable]
    public class CardStat
    {
        public StatType statType;
        public int statValue;

        public System.Action OnStatChange;
    }


    [System.Serializable]
    public abstract class CardAction : ScriptableObject
    {
        public string actionName;
        public string actionDescription;
        public int actionCost;


        [Space]
        public ActionType actionType;
        public ActionPriority actionPriority;
        public int actionValue;
        public ITargetableCard targetCard;


        public virtual bool CanAffordAction(int resources) { return resources >= actionCost; }
        public virtual void ProcessActionCost(ref int resources) { resources -= actionCost; }
        public abstract void ProcessActionOnTargets();
    }


    [CreateAssetMenu(fileName = "MonsterCardData", menuName = "Cards/CardDatas/MonsterCardData")]
    public class MonsterCardData : CardData
    {
        public override CardType GetCardType () => CardType.MONSTER;
    }




    [CreateAssetMenu(fileName = "AttackCardAction", menuName = "Cards/CardActions/AttackAction")]
    public class AttackCardAction : CardAction
    {
        public override void ProcessActionOnTargets()
        {
            targetCard.AdjustStatBy(StatType.HEALTH, -actionValue);
            Debug.Log(string.Format("{0}: {1}", targetCard, targetCard.GetCardStat(StatType.HEALTH)));
        }
    }


    [CreateAssetMenu(fileName = "HealCardAction", menuName = "Cards/CardActions/HealAction")]
    public class HealCardAction : CardAction
    {
        public override void ProcessActionOnTargets()
        {
            targetCard.AdjustStatBy(StatType.HEALTH, actionValue);
            Debug.Log(string.Format("{0}: {1}", targetCard, targetCard.GetCardStat(StatType.HEALTH).statValue));
        }
    }




    public interface ITargetableCard
    {
        CardType GetCardType();
        CardStat GetCardStat(StatType type);
        int AdjustStatBy(StatType stat, int deltaStat);
    }


    public enum CardType
    {
        VOID,
        HERO,       // Player
        MONSTER,
        SPELL,
    }

    public enum StatType
    {
        VOID,
        COST,
        HEALTH,
        RESOURCE,
    }

    public enum ActionType
    {
        VOID,
        MOVE,
        ATTACK,
        DEFEND,
        HEAL,
        BUFF,
    }

    public enum ActionPriority
    {
        VOID,
        PRIMARY,
        SECONDARY,
        ACTIVE,
        PASSIVE,
        SPECIAL,
    }
}