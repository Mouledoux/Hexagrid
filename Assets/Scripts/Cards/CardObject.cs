using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Cards
{
    public class CardObject : MonoBehaviour, ITargetableCard
    {
        public CardData cardData;


        [Space]
        private string cardName;
        private CardType cardType;


        public Dictionary<StatType, CardStat> cardStats;
        public Dictionary<ActionPriority, CardAction> cardActions;


        [SerializeField]
        private SpriteRenderer cardArtwork;
        [SerializeField]
        private TMPro.TextMeshPro cardDescription;


        public CardObject(){}

        public CardObject(CardData cardData) =>
            new CardObject(cardData.cardName, cardData.GetCardType(), cardData.cardStats, cardData.cardActions);
                    
        public CardObject(string name, CardType type, CardStat[] stats, CardAction[] actions)
        {
            cardName = name;
            cardType = type;

            cardStats = new Dictionary<StatType, CardStat>();
            foreach(CardStat cs in stats)
                cardStats.Add(cs.statType, cs);

            cardActions = new Dictionary<ActionPriority, CardAction>();
            foreach(CardAction ca in actions)
                cardActions.Add(ca.actionPriority, ca);
        }

        public void Initialize(CardData cardData)
        {
            cardName = cardData.cardName;
            cardType = cardData.GetCardType();
            
            cardStats = new Dictionary<StatType, CardStat>();
            foreach(CardStat cs in cardData.cardStats)
                cardStats.Add(cs.statType, cs);

            cardActions = new Dictionary<ActionPriority, CardAction>();
            foreach(CardAction ca in cardData.cardActions)
                cardActions.Add(ca.actionPriority, ca);

            cardArtwork.sprite = cardData.cardArtwork;
            cardDescription.text = cardData.cardDescription;
        }


        private void Awake()
        {
            Initialize(cardData);
        }


        CardType ITargetableCard.GetCardType() => cardType;
        CardStat ITargetableCard.GetCardStat(StatType type) => cardStats[type];
        int ITargetableCard.AdjustStatBy(StatType type, int deltaStat)
        {
            cardStats[type].statValue += deltaStat;
            cardStats[type].OnStatChange?.Invoke();
            return cardStats[type].statValue;
        }
    }

}

