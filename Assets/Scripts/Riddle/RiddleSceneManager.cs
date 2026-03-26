using System;
using System.Collections;
using System.Linq;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Riddle {
    [DefaultExecutionOrder(-996)]
    public class RiddleSceneManager : MonoBehaviour {

        [SerializeField]
        private SfxHandler sfxHandler;
        [SerializeField]
        private TextMeshProUGUI riddleText;
        [SerializeField]
        private TextMeshProUGUI riddleExitText;
        [SerializeField]
        private TextMeshProUGUI riddleIdText;
        [SerializeField]
        private TextMeshProUGUI timerText;
        [SerializeField]
        private GameObject tutorialText;
        [SerializeField]
        private WordHolderController wordHolder;
        [SerializeField]
        private CardMover cardMover;
        [SerializeField]
        private StartButtonController startButton;

        [Header("Debug")]
        [SerializeField]
        [Tooltip("Offset the days from today to test upcoming riddles.")]
        private int debugDays = 0;
        [SerializeField]
        private bool isForScreenshot = false;
        [SerializeField]
        private bool showCards = false;
        [SerializeField]
        private CardController cardPrefab;

        private RiddleRoundPresenter _riddleRoundPresenter = null;
        private bool _isRiddleWon = false;
        private string _currentRiddle = "";
        private string _currentAnswer = "";
        private int _currentRiddleId = 0;
        private double _elapsedTime = 0.0;

        private static IEnumerator SolveRiddleOnServer(double elapsedTime, Action<string> jsonCallback) {
            var jsonBody = "{ \"elapsedTime\": " + elapsedTime.ToString("F2") + "}";
            using (var req = UnityWebRequest.Post("/api/riddle-solved", jsonBody, "application/json")) {
                yield return req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.Success) {
                    var json = req.downloadHandler.text;
                    jsonCallback?.Invoke(json);
                }
                else {
                    jsonCallback?.Invoke("{ \"status\": \"failed\", \"message\": \"UnityWebRequest failed.\" }");
                }
            }
        }

        public void StartGame() {
#if !UNITY_EDITOR
            debugDays = 0;
            isForScreenshot = false;
#endif
            StartCoroutine(RiddleProvider.GetRiddleOfTheDay(debugDays, (item) => {
                _isRiddleWon = false;
                _currentRiddle = item.riddle;
                _currentAnswer = item.answer;
                _currentRiddleId = item.riddleId;

                riddleIdText.text = $"RIDDLE #{_currentRiddleId}";
                riddleText.text = _currentRiddle;
                riddleExitText.text = $"The answer is <b>{_currentAnswer.ToUpper()}</b>.\nSee you tomorrow for a new riddle!";
                _riddleRoundPresenter = new RiddleRoundPresenter(startButton, wordHolder, tutorialText, timerText.gameObject, riddleIdText.gameObject, riddleText, _currentAnswer, this);

                // Listen first before setting the answer so the events of wordHolder will invoke
                // sfxHandler's methods.
                sfxHandler.ListenForSfx(startButton, _riddleRoundPresenter, wordHolder);
                wordHolder.SetAnswer(_currentAnswer);

                _riddleRoundPresenter.ProcessState();
                _elapsedTime = -1.0;
                AnalyticsManager.StartLevel();

                OnClickStartRound();
            }));
        }

        public void OnClickStartRound() {
            _riddleRoundPresenter?.RunNextState();
        }

        public void OnRiddleSolved(Action<RiddleSolvedData> onFinished) {
            StartCoroutine(OnRiddleSolvedRoutine(onFinished));
        }

        private IEnumerator OnRiddleSolvedRoutine(Action<RiddleSolvedData> onFinished) {
            yield return SolveRiddleOnServer(_elapsedTime, (json) => {
                Debug.Log("JSON RETURN: " + json);
                var data = JsonUtility.FromJson<RiddleSolvedData>(json);
                onFinished?.Invoke(new RiddleSolvedData() { status = data.status, message = data.message });
            });
        }

        private void OnWordChanged(string word) {
#if UNITY_EDITOR
            Debug.Log(word);
#endif
            if (word.ToUpper() == _currentAnswer.ToUpper()) {
                // Win
                if (isForScreenshot)
                {
                    return;
                }
                cardMover.enabled = false;
                _isRiddleWon = true;
                _riddleRoundPresenter.RunWinStates(() => {
                    OnRiddleSolved((result) => {
                        riddleExitText.gameObject.SetActive(true);
                    });
                });

                AnalyticsManager.CompleteLevel((float)_elapsedTime);
                //_elapsedTime = 0.0;
            }
        }

        private void OnEnable() {
            wordHolder.OnWordChanged += OnWordChanged;
        }

        private void OnDisable() {
            wordHolder.OnWordChanged -= OnWordChanged;
        }

        private void Start() {
            StartGame();
            if (showCards) {
                DebugCards();
            }
        }

        private void Update() {
            if (!_isRiddleWon) {
                _elapsedTime += (double)Time.deltaTime;
                timerText.text = TimeSpan.FromSeconds(_elapsedTime).ToString(@"mm\:ss");
            }
        }

        private void DebugCards() {
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int count = letters.Length;
            var colCount = 6;
            var intervalX = 0.8f;
            var intervalY = 1.2f;
            var startPosX = intervalX * colCount * -0.5f;
            var posX = startPosX;
            var posY = intervalY * (count / colCount) * -0.5f; ;
            int i = 0;

            for (int y = 0; y <= (count / colCount) + 1; y++) {
                for (int x = 0; x <= colCount; x++) {
                    var g = Instantiate(cardPrefab, null, false);
                    g.transform.localScale = Vector3.one * 0.4f;
                    g.SetLetter(letters[i++]);
                    g.FlipOpen(true);
                    g.transform.position = new Vector3(posX, posY, -4f);
                    posX += intervalX;

                    if (i >= count) {
                        break;
                    }
                }
                posX = startPosX;
                posY += intervalY;
                if (i >= count) {
                    break;
                }
            }
        }
    }
}
