
namespace Common.Analytics {
    public class AnalyticsLevelEvent : Unity.Services.Analytics.Event {
        public AnalyticsLevelEvent(string eventName) : base(eventName) {
        }

        public string Riddle { set { SetParameter("riddle", value); } }
        public string Answer { set { SetParameter("answer", value); } }
        public int RiddleIndex { set { SetParameter("riddleIndex", value); } }
        public float TimeTaken { set { SetParameter("timeTaken", value); } }
    }
}
