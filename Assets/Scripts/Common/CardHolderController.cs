using System;
using DG.Tweening;
using UnityEngine;

namespace Common {
    [RequireComponent(typeof(BoxCollider))]
    public class CardHolderController : MonoBehaviour {

        public static readonly Vector2 Size = new Vector2(0.53f, 0.75f);

        private const float SnapDuration = 0.1f;

        [SerializeField]
        private Transform slot;

        public CardController Card { private set; get; } = null;
        public bool IsSnapping { private set; get; } = false;
        public Transform Slot => slot;

        public event Action<CardHolderController> OnValueChanged;

        public void Clear() {
            Card = null;
            IsSnapping = false;
        }

        public void HoldSilently(CardController card) {
            Card = card;
        }

        public void Hold(CardController card, bool snap = true) {
            if (IsSnapping) {
                return;
            }

            card.transform.SetParent(slot);
            if (snap) {
                card.transform.localPosition = Vector3.zero;
                Card = card;
                Card.ReleaseByCursor();
                OnValueChanged?.Invoke(this);
                return;
            }

            IsSnapping = true;
            var duration = SnapDuration;
            card.transform.DOLocalMove(Vector3.zero, duration).SetEase(Ease.Linear).OnComplete(() => {
                card.transform.localPosition = Vector3.zero;
                Card = card;
                Card.ReleaseByCursor();
                OnValueChanged?.Invoke(this);
                IsSnapping = false;
            });
        }
    }
}
