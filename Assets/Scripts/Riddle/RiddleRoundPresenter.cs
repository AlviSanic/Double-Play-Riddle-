using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using Common;

namespace Riddle {
    public class RiddleRoundPresenter {

        private StartButtonController _startButton;
        private WordHolderController _wordHolder;
        private TextMeshProUGUI _riddleText;
        private GameObject _tutorialText;
        private GameObject _timerText;
        private GameObject _riddleIdText;
        private RiddleSceneManager _sceneManager;
        private RoundPresenterState _state;
        private string _answer;
        private Action _runNextState;
        private Action _onFinishedWinStates;

        public event Action<CardController> OnCardLift;
        public event Action<CardController> OnCardFlip;
        public event Action OnShowRiddle;
        public event Action<int> OnWinCardJump;
        public event Action OnWinCollectionCard;
        public event Action OnWinCollectionExit;

        public RiddleRoundPresenter(StartButtonController startButton, WordHolderController wordHolder, GameObject tutorialText, GameObject timerText, GameObject riddleIdText, TextMeshProUGUI riddleText, string answer, RiddleSceneManager sceneManager) {
            _startButton = startButton;
            _wordHolder = wordHolder;
            _riddleText = riddleText;
            _tutorialText = tutorialText;
            _timerText = timerText;
            _riddleIdText = riddleIdText;
            _sceneManager = sceneManager;
            _answer = answer;
            _state = RoundPresenterState.PreStart;
        }

        public void RunWinStates(Action onFinished) {
            _onFinishedWinStates = onFinished;
            _state = RoundPresenterState.CollectForPresentation;
            ProcessState();
        }

        public void RunNextState() {
            _runNextState?.Invoke();
        }

        public void ProcessState() {
            if (_state == RoundPresenterState.End) {
                return;
            }

            if (_state == RoundPresenterState.PreStart) {
                _sceneManager.StartCoroutine(RunPreStartRoutine());
            }
            else if (_state == RoundPresenterState.Start) {
                _sceneManager.StartCoroutine(RunStartRoutine());
            }
            else if (_state == RoundPresenterState.DropCards) {
                _sceneManager.StartCoroutine(RunDropCardsRoutine());
            }
            else if (_state == RoundPresenterState.ShowRiddle) {
                _sceneManager.StartCoroutine(RunShowRiddleRoutine());
            }
            else if (_state == RoundPresenterState.FlipCards) {
                _sceneManager.StartCoroutine(RunFlipCardsRoutine());
            }
            else if (_state == RoundPresenterState.CollectForPresentation) {
                _sceneManager.StartCoroutine(RunCollectForPresentationRoutine());
            }
        }

        private IEnumerator RunPreStartRoutine() {
            // Don't show the play button anymore.
            //_startButton.gameObject.SetActive(true);
            _startButton.OnDoneExitAnimation += OnButtonDoneExitAnimation;

            var color = _riddleText.color;
            _riddleText.color = new Color(color.r, color.g, color.b, 0f);

            _wordHolder.ClearChildren();
            _wordHolder.SetAnswer(_answer);
            _wordHolder.PrepareForEntrance();

            _runNextState = () => {
                _runNextState = null;
                _state = RoundPresenterState.Start;
                ProcessState();
            };
            yield break;
        }

        private void OnButtonDoneExitAnimation() {
            RunNextState();
            _startButton.gameObject.SetActive(false);
            _startButton.OnDoneExitAnimation -= OnButtonDoneExitAnimation;
        }

        private IEnumerator RunStartRoutine() {
            var proceed = false;
            _wordHolder.RunEntrance((c) => {
                OnCardLift?.Invoke(c);
            }, () => {
                proceed = true;
            });
            yield return new WaitUntil(() => proceed);

            _state = RoundPresenterState.DropCards;
            yield return null;
            ProcessState();
        }

        private IEnumerator RunDropCardsRoutine() {
            _state = RoundPresenterState.ShowRiddle;
            yield return null;
            ProcessState();
        }

        private IEnumerator RunShowRiddleRoutine() {
            var pos = _riddleText.transform.position;
            _riddleText.transform.position = new Vector3(pos.x, pos.y + 40f, pos.z);

            var color = _riddleText.color;
            _riddleText.DOColor(new Color(color.r, color.g, color.b, 1f), 1f);
            _riddleText.transform.DOMoveY(pos.y, 1f).SetEase(Ease.OutBack);
            OnShowRiddle?.Invoke();
            yield return new WaitForSeconds(0.4f);

            _state = RoundPresenterState.FlipCards;
            yield return null;
            ProcessState();
        }

        private IEnumerator RunFlipCardsRoutine() {
            foreach (var ch in _wordHolder.CardHolders) {
                ch.Card?.FlipOpen(false);
                OnCardFlip?.Invoke(ch.Card);
                yield return new WaitForSeconds(0.1f);
            }
            _tutorialText.SetActive(true);
            _timerText.SetActive(true);
            _riddleIdText.SetActive(true);
            _wordHolder.SetWordDetection(true);
            _state = RoundPresenterState.End;
            yield return null;
            ProcessState();
        }

        private IEnumerator RunCollectForPresentationRoutine() {
            _tutorialText.SetActive(false);

            var proceed = false;
            _wordHolder.RunCollection((i) => {
                OnWinCardJump?.Invoke(i);
            }, () => {
                OnWinCollectionCard?.Invoke();
            }, () => {
                OnWinCollectionExit?.Invoke();
                proceed = true;
            });
            yield return new WaitUntil(() => proceed);

            _state = RoundPresenterState.End;
            yield return null;
            _onFinishedWinStates?.Invoke();
            _onFinishedWinStates = null;
            ProcessState();
        }
    }
}
