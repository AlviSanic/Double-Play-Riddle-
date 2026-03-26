using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Riddle {
    public class SfxHandler : MonoBehaviour {

        [SerializeField]
        private CardMover cardMover;

        private StartButtonController _startButton;
        private RiddleRoundPresenter _riddleRoundPresenter;
        private WordHolderController _wordHolder;
        private IReadOnlyList<CardController> _cards;
        private Dictionary<string, float> _cooldowns = new();

        public void ListenForSfx(StartButtonController startButton, RiddleRoundPresenter riddleRoundPresenter, WordHolderController wordHolder) {
            _startButton = startButton;
            _startButton.OnStackDeck += OnStartButtonStackDeck;
            _startButton.OnFlipCard += OnStartButtonFlipCard;
            _startButton.OnExitDeck += OnStartButtonExitDeck;
            _riddleRoundPresenter = riddleRoundPresenter;
            riddleRoundPresenter.OnCardLift += OnEntranceCardLift;
            riddleRoundPresenter.OnCardFlip += OnEntranceCardFlip;
            riddleRoundPresenter.OnShowRiddle += OnEntranceShowRiddle;
            riddleRoundPresenter.OnWinCardJump += OnWinCardJump;
            riddleRoundPresenter.OnWinCollectionCard += OnWinCollectionCard;
            riddleRoundPresenter.OnWinCollectionExit += OnWinCollectionExit;
            _wordHolder = wordHolder;
            _wordHolder.OnCardsSpawned += OnCardsSpawned;
            _wordHolder.OnCardShifted += OnCardShifted;
        }

        private void OnCardsSpawned(IReadOnlyList<CardController> cards) {
            _cards = cards;
            foreach (var c in _cards) {
                c.OnRotateStart += OnCardRotateStart;
                c.OnRotated += OnCardRotated;
            }
        }

        private void OnStartButtonStackDeck() {
            PlaySfx("sfxCardSettle", 0.6f);
        }

        private void OnStartButtonFlipCard() {
            PlaySfx("sfxCardSlide", 0.6f);
        }

        private void OnStartButtonExitDeck() {
            PlaySfx("sfxCardRotate", 0.8f);
        }

        private void OnEntranceShowRiddle() {
            PlaySfx("sfxPop05", 0.3f);
        }

        private void OnEntranceCardLift(CardController card) {
            PlaySfx("sfxCardRotate", 0.4f);
        }

        private void OnEntranceCardFlip(CardController card) {
            PlaySfx("sfxCardSlide", 0.4f);
        }

        private void OnCardRotateStart(CardController card) {
            PlaySfx("sfxCardRotate", 0.8f);
        }

        private void OnCardRotated(CardController card) {
        }

        private void OnCardDrag(CardController card, CardHolderController holder) {
            //PlaySfx("sfxCardSlide", 1.0f);
        }

        private void OnCardHold(CardController card, CardHolderController holder) {
            PlaySfx("sfxCardSlide", 0.8f);
        }

        private void OnCardShifted(CardController card, CardHolderController holder) {
            PlaySfx("sfxCardSlide", 0.6f);
        }

        private void OnWinCardJump(int i) {
            switch (i) {
                case 0:
                    PlaySfx("sfxPop01", 0.3f);
                    break;
                case 1:
                    PlaySfx("sfxPop02", 0.3f);
                    break;
                case 2:
                    PlaySfx("sfxPop03", 0.4f);
                    break;
                case 3:
                    PlaySfx("sfxPop04", 0.4f);
                    break;
                case 4:
                    PlaySfx("sfxPop05", 0.5f);
                    break;
            }
        }

        private void OnWinCollectionCard() {
            PlaySfx("sfxCardRotate", 0.6f);
        }

        private void OnWinCollectionExit() {
            PlaySfx("sfxCardRotate", 0.6f);
        }

        private void Start() {
            cardMover.OnCardDrag += OnCardDrag;
            cardMover.OnCardHold += OnCardHold;
        }

        private void Update() {
            if (_cooldowns != null) {
                var cds = new Dictionary<string, float>(_cooldowns);
                foreach (var key in cds.Keys) {
                    _cooldowns[key] -= Time.deltaTime;
                    if (_cooldowns[key] <= 0f) {
                        _cooldowns.Remove(key);
                    }
                }
            }
        }

        private void OnDestroy() {
            _riddleRoundPresenter.OnCardLift -= OnEntranceCardLift;
            _riddleRoundPresenter.OnCardFlip -= OnEntranceCardFlip;
            cardMover.OnCardDrag -= OnCardDrag;
            cardMover.OnCardHold -= OnCardHold;

            if (_wordHolder != null) {
                foreach (var c in _cards) {
                    c.OnRotated -= OnCardRotated;
                }
                _wordHolder.OnCardsSpawned -= OnCardsSpawned;
                _wordHolder.OnCardShifted -= OnCardShifted;
                _wordHolder = null;
            }
        }

        private void PlaySfx(string clipName, float volume = 1f) {
            if (_cooldowns.ContainsKey(clipName)) {
                return;
            }
            AudioManager.PlaySfx(clipName, volume);
            _cooldowns[clipName] = 0.05f;
        }
    }
}
