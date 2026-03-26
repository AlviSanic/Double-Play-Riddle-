using UnityEngine;

namespace Utility {
    [DefaultExecutionOrder(-999)]
    public class WordFinderLoader : MonoBehaviour {

        [SerializeField]
        private TextAsset enableList;

        private void Start() {
            WordFinder.CreateInstance(enableList.text);
        }
    }
}
