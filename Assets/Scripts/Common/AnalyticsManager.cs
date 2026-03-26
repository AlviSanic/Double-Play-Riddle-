using Common.Analytics;
using Riddle;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Common {
    [DefaultExecutionOrder(-998)]
    public class AnalyticsManager : MonoBehaviour {

        public static void StartLevel() {
            instance?.OnLevelStart();
        }

        public static void CompleteLevel(float timeTaken) {
            instance?.OnLevelComplete(timeTaken);
        }

        private static AnalyticsManager instance = null;

        private void Awake() {
            if (instance != null) {
                DestroyImmediate(gameObject);
            }
            instance = this;
        }

        private async void Start() {
            //await UnityServices.InitializeAsync();
            //AnalyticsService.Instance.StartDataCollection();
        }

        private void OnLevelStart() {
            // var riddle = RiddleProvider.GetRiddleOfTheDay(this, 0, out var index);
            // var e = new AnalyticsLevelEvent("riddle_start");
            // e.Riddle = riddle.riddle;
            // e.Answer = riddle.answer;
            // e.RiddleIndex = index;
            // AnalyticsService.Instance.RecordEvent(e);
        }

        private void OnLevelComplete(float timeTaken) {
            // var riddle = RiddleProvider.GetRiddleOfTheDay(this, 0, out var index);
            // var e = new AnalyticsLevelEvent("riddle_complete");
            // e.Riddle = riddle.riddle;
            // e.Answer = riddle.answer;
            // e.RiddleIndex = index;
            // e.TimeTaken = timeTaken;
            // AnalyticsService.Instance.RecordEvent(e);
        }
    }
}
