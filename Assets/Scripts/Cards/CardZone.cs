using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class CardZone : MonoBehaviour
    {
        private bool initialised = false;
        protected List<CardObject> cards;

        [SerializeField]
        protected UnityEngine.Events.UnityAction onCardAddUnityAction;

        [SerializeField]
        protected UnityEngine.Events.UnityAction onCardRemoveUnityAction;


        public System.Action onCardAdd;
        public System.Action onCardRemove;


        public void Initialize()
        {
            if(!initialised)
            {
                onCardAdd += onCardAddUnityAction.Invoke;
                onCardRemove += onCardRemoveUnityAction.Invoke;
            }
        }


        protected void AddCard(CardObject newCard)
        {
            cards.Add(newCard);
            onCardAdd?.Invoke();
        }

        protected CardObject RemoveCard(CardObject oldCard)
        {
            if(cards.Contains(oldCard))
            {
                cards.Remove(oldCard);
                onCardRemove?.Invoke();
                return oldCard;
            }
            else throw new System.ArgumentException();
        }

        public void MoveCardToNewZone(CardObject cardObject, CardZone newZone)
        {
            if(!cards.Contains(cardObject))
            {
                throw new System.ArgumentException();
            }
            
            else
            {
                newZone.AddCard(RemoveCard(cardObject));
            }
        }
    }
}
