using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Riddle {
    public class WordHolderController : MonoBehaviour {

        [SerializeField]
        private CardMover cardMover;
        [SerializeField]
        private CardController cardPrefab;
        [SerializeField]
        private CardHolderController cardHolderPrefab;

        public event Action<IReadOnlyList<CardController>> OnCardsSpawned;
        public event Action<IReadOnlyList<CardHolderController>> OnCardHoldersSpawned;
        public event Action<CardController, CardHolderController> OnCardShifted;
        public event Action<string> OnWordChanged;

        public IReadOnlyList<CardHolderController> CardHolders => _cardHolders;

        private string _answer = "";
        private List<CardHolderController> _cardHolders = new();
        private List<CardController> _cards = new();
        private string _detectedWord = "";
        private bool _isDetecingWordChanges = false;

        private Dictionary<CardHolderController, (CardHolderController left, CardHolderController right)> _neighborHolderLookup = new();

        public void SetAnswer(string answer) {
            if (answer.Trim().Length < 3) {
                Debug.LogError($"WordHolder should hold an answer that has 3 or more letters. \"{answer}\" is not a valid answer.");
                return;
            }
            _answer = answer;
            ClearChildren();
            InstantiateCardHolders(_answer);
            InstantiateCards(_answer);
            UpdateHolderLookup();
        }

        public void ClearChildren() {
            foreach (Transform t in transform) {
                if (t == transform) {
                    continue;
                }
                Destroy(t.gameObject);
            }

            _cards.Clear();
            _cardHolders.Clear();
            _neighborHolderLookup.Clear();
        }

        public void SetWordDetection(bool enabled) {
            _isDetecingWordChanges = enabled;
        }

        public void PrepareForEntrance() {
            foreach (var ch in _cardHolders) {
                var pos = ch.transform.position;
                ch.transform.position = new Vector3(pos.x, pos.y - 4f, pos.z);
            }
        }

        public void RunEntrance(Action<CardController> onCardLift, Action onFinished) {
            StartCoroutine(RunEntranceRoutine(onCardLift, onFinished));
        }

        public void RunCollection(Action<int> onCardJump, Action onCardStack, Action onFinished) {
            StartCoroutine(RunCollectionRoutine(onCardJump, onCardStack, onFinished));
        }

        private IEnumerator RunEntranceRoutine(Action<CardController> onCardLift, Action onFinished) {
            int i = 0;
            foreach (var ch in _cardHolders) {
                var pos = ch.transform.position;
                ch.transform.position = new Vector3(pos.x, pos.y - 4f, pos.z);
                var tween = ch.transform.DOLocalMoveY(0f, 0.4f).SetEase(Ease.OutBack);
                onCardLift?.Invoke(ch.Card);
                if (i == _cardHolders.Count - 1) {
                    tween.OnComplete(() => {
                        onFinished?.Invoke();
                    });
                }
                yield return new WaitForSeconds(0.05f);
                i++;
            }
        }

        private IEnumerator RunCollectionRoutine(Action<int> onCardJump, Action onCardStack, Action onFinished) {
            if (_cardHolders.Count <= 0) {
                yield break;
            }
            SetWordDetection(false);
            var leftPos = _cardHolders[0].transform.position;
            var rightPos = _cardHolders[_cardHolders.Count - 1].transform.position;
            var leftToRightDistance = rightPos.x - leftPos.x;
            var duration = 0.1f;

            int i = 0;
            foreach (var ch in _cardHolders) {
                ch.transform.DOMoveY(leftPos.y + 0.4f, duration * 1.4f).SetEase(Ease.Linear).OnComplete(() => {
                    ch.transform.DOMoveY(leftPos.y, duration * 1.4f).SetEase(Ease.Linear);
                });
                onCardJump?.Invoke(i);
                yield return new WaitForSeconds(duration);
                i++;
            }
            yield return new WaitForSeconds(duration * 15.0f);

            i = 0;
            onCardStack?.Invoke();
            foreach (var ch in _cardHolders) {
                ch.transform.DOMoveZ(leftPos.z - (i * 0.4f), 0.1f);
                ch.transform.DOMoveX(leftPos.x + (leftToRightDistance * 0.5f), duration * 1.4f).SetEase(Ease.Linear);
                i++;
            }
            yield return new WaitForSeconds(duration * 1.6f);

            /*foreach (var ch in _cardHolders) {
                ch.transform.DOMoveX(basePos.x - 0.3f, duration * 0.4f).SetEase(Ease.Linear).OnComplete(() => {
                    ch.transform.DOMoveX(basePos.x, duration * 0.4f).SetEase(Ease.Linear);
                });
            }*/
            //yield return new WaitForSeconds(duration * 0.4f * 8f);

            foreach (var ch in _cardHolders) {
                ch.transform.DOMoveY(leftPos.y - 4f, duration * 1.4f).SetEase(Ease.InBack);
            }
            yield return new WaitForSeconds(duration * 2.2f);
            onFinished?.Invoke();
        }

        private void InstantiateCardHolders(string answer) {
            var startingPosX = (answer.Length * -0.5f * CardHolderController.Size.x) + (CardHolderController.Size.x * 0.5f);
            var posX = startingPosX;
            foreach (var c in answer) {
                var g = Instantiate(cardHolderPrefab, transform, false);
                g.transform.localPosition = new Vector3(posX, 0f, 0f);
                posX += CardHolderController.Size.x;
                _cardHolders.Add(g);
            }
            OnCardHoldersSpawned?.Invoke(_cardHolders);
        }

        private void InstantiateCards(string answer) {
            var holders = new List<CardHolderController>(_cardHolders);
            foreach (var c in answer) {
                var holder = holders[Random.Range(0, holders.Count)];
                var g = Instantiate(cardPrefab, null, false);
                g.SetLetter(Random.Range(0, 2) == 0 ? c : CardPairs.GetPair(c));
                g.FlipClose(true);
                g.transform.position = holder.Slot.position;

                holders.Remove(holder);
                holder.Hold(g);
                _cards.Add(g);
            }
            OnCardsSpawned?.Invoke(_cards);
        }

        private void UpdateHolderLookup() {
            for (var i = 0; i < _cardHolders.Count; i++) {
                var holder = _cardHolders[i];
                CardHolderController left = i <= 0 ? null : _cardHolders[i - 1];
                CardHolderController right = i >= _cardHolders.Count - 1 ? null : _cardHolders[i + 1];
                _neighborHolderLookup[holder] = (left, right);
            }
        }

        /// <summary>
        /// Determine whether a certain card hold is allowed or not.
        /// </summary>
        /// <param name="card">The card that is about to be held.</param>
        /// <param name="cardHolder">The potential holder of the card.</param>
        /// <returns>Return false to disallow the card hold.</returns>
        private bool OnValidateCardHold(CardController card, CardHolderController cardHolder) {
            return true;
        }

        private bool OnShiftCards(CardHolderController cardHolder, int direction, bool shiftToFallback) {
            if (cardHolder.Card == null) {
                return true;
            }

            var cardHolders = new List<CardHolderController>() { cardHolder };
            if (direction > 0 && !shiftToFallback) {
                if (!FindEmptyCardHolder(cardHolder, 1, cardHolders)) {
                    return false;
                }
                // Shift cards to right
                ShiftCards(cardHolders);
            }
            else if (direction < 0 && !shiftToFallback) {
                if (!FindEmptyCardHolder(cardHolder, -1, cardHolders)) {
                    return false;
                }
                // Shift cards to left
                ShiftCards(cardHolders);
            }
            else {
                var dir = 1;
                if (direction < 0) {
                    dir = -1;
                }

                if (FindEmptyCardHolder(cardHolder, dir, cardHolders)) {
                    ShiftCards(cardHolders);
                }
                else {
                    cardHolders.Clear();
                    cardHolders.Add(cardHolder);
                    if (!FindEmptyCardHolder(cardHolder, -dir, cardHolders)) {
                        return false;
                    }
                    ShiftCards(cardHolders);
                }
            }

            return true;
        }

        private void ShiftCards(List<CardHolderController> cardHolders) {
            if (cardHolders.Count < 1) {
                return;
            }
            OnCardShifted?.Invoke(cardHolders[0].Card, cardHolders[1]);

            cardHolders.Reverse();
            CardHolderController prev = null;
            foreach (var ch in cardHolders) {
                if (prev == null) {
                    prev = ch;
                    continue;
                }
                prev.Hold(ch.Card, false);
                ch.Clear();
                prev = ch;
            }
        }

        private bool FindEmptyCardHolder(CardHolderController cardHolder, int step, List<CardHolderController> cardHolders) {
            if (step > 0) {
                var right = _neighborHolderLookup[cardHolder].right;
                cardHolders.Add(right);
                if (right == null || right.IsSnapping) {
                    return false;
                }
                else if (right.Card == null) {
                    return true;
                }
                return FindEmptyCardHolder(right, step, cardHolders);
            }
            else if (step < 0) {
                var left = _neighborHolderLookup[cardHolder].left;
                cardHolders.Add(left);
                if (left == null || left.IsSnapping) {
                    return false;
                }
                else if (left.Card == null) {
                    return true;
                }
                return FindEmptyCardHolder(left, step, cardHolders);
            }
            return false;
        }

        private void OnEnable() {
            cardMover.onValidateCardHold = OnValidateCardHold;
            cardMover.onShiftCards = OnShiftCards;
        }

        private void OnDisable() {
            cardMover.onValidateCardHold = null;
            cardMover.onShiftCards = OnShiftCards;
        }

        private void Update() {
            DetectWordChanges();
        }

        private void DetectWordChanges() {
            if (!_isDetecingWordChanges) {
                return;
            }

            var word = "";
            foreach (var ch in _cardHolders) {
                // The sequence of these if conditions is important to avoid
                // detecting incomplete words
                if (ch.IsSnapping) {
                    word = "";
                    break;
                }
                if (ch.Card == null) {
                    break;
                }
                if (ch.Card.IsRotating || ch.Card.IsHeldByCursor) {
                    word = "";
                    break;
                }
                word += ch.Card.Letter;
            }
            if (word != "" && word.ToUpper() != _detectedWord.ToUpper()) {
                _detectedWord = word.ToUpper();
                OnWordChanged?.Invoke(_detectedWord);
            }
        }
    }
}
