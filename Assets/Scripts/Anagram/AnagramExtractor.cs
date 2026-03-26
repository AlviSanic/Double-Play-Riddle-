using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utility;
using System.Linq;
using System;


#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif

namespace Anagram {
    public class AnagramExtractor : MonoBehaviour {

        [SerializeField]
        private CardLetterSprites cards;
        [SerializeField]
        [Min(1)]
        [Tooltip("1-based")]
        private int startCombination;
        [SerializeField]
        private int combinationCount;
        [SerializeField]
        [Min(1)]
        private int minCardCount = 3;
        [SerializeField]
        private int cardCount = 5;

#if UNITY_EDITOR
        [ContextMenu("Extract All Words")]
        public void ExtractAllWords() {
            var enableList = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Data/enablelist.txt");
            WordFinder.CreateInstance(enableList.text);

            var path = Path.Combine(Application.persistentDataPath, "anagramWords.txt");
            Debug.Log($"Saving at {path}");
            EditorCoroutineUtility.StartCoroutine(GetAllWordsRoutine(path), this);
            Debug.Log($"Done saving at {path}");
        }
#endif

        private IEnumerator GetAllWordsRoutine(string path) {
            cardCount = Math.Max(cardCount, minCardCount);

            var str = "Double Play Anagram Words\n";
            str += $"COMBOS\t{minCardCount}-LETTER WORD COUNT\t{minCardCount}-LETTER WORDS";
            for (int i = 0; i < cardCount - minCardCount; i++) {
                str += $"\t{i + (minCardCount + 1)}-LETTER WORD COUNT\t{i + (minCardCount + 1)}-LETTER WORDS";
            }
            str += "\n";

            File.Delete(path);
            var stream = new StreamWriter(path);

            var uniqueChars = GetUniqueChars();
            Debug.Log($"There are {uniqueChars.Count} unique characters.");

            int comboCount = 0;
            var combos = GetLetterCombinationsWithRepetition(uniqueChars, cardCount);
            //str += $"Combo Count from {cardCount} cards: : {combos.Count}\n\n";
            for (int ci = startCombination - 1; comboCount < combinationCount; ci++) {
                if (ci >= combos.Count) {
                    break;
                }
                var combo = combos[ci];
                comboCount++;
                str += $"{combo}";

                for (int i2 = 0; i2 < cardCount - (minCardCount - 1); i2++) {
                    int i3 = 0;
                    var words = GetWords(combo, i2 + minCardCount);
                    str += $"\t{words.Length}\t";
                    foreach (var word in words) {
                        str += i3 > 0 ? ", " : "";
                        str += $"{word}";
                        i3++;
                    }
                    yield return null;
                }
                str += "\n";

                if (comboCount % 100 == 0 || comboCount == combos.Count) {
                    Debug.Log($"Combo({comboCount}): {combo}\n");
                }
            }

            stream.Write(str);
            stream.Close();
        }

        private string[] GetWords(string anagram, int letterCount) {
            var words = WordFinder.Instance.FindDoublePlayWords(anagram, letterCount, "", 0);
            return words;
        }

        private List<string> GetLetterCombinationsWithRepetition(List<char> chars, int comboLength) {
            if (chars == null || chars.Count == 0 || comboLength <= 0) {
                return null;
            }

            chars.Sort(); // keep combos in a predictable order
            var buffer = new char[comboLength];
            var allCombos = new List<string>();
            foreach (var combo in Build(0, 0, comboLength, buffer, chars)) {
                allCombos.Add(combo);
            }
            return allCombos;
        }

        private IEnumerable<string> Build(int startIndex, int depth, int comboLength, char[] buffer, List<char> chars) {
            if (depth == comboLength) {
                yield return new string(buffer);
                yield break;
            }

            for (int i = startIndex; i < chars.Count; i++) {
                buffer[depth] = chars[i];
                foreach (var combo in Build(i, depth + 1, comboLength, buffer, chars)) {
                    yield return combo;
                }
            }
        }


        private List<char> GetUniqueChars() {
            var uniqueChars = new Dictionary<char, bool>();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            foreach (var c in chars) {
                var main = c;
                var pair = CardPairs.GetPair(c);
                if (!uniqueChars.ContainsKey(pair)) {
                    uniqueChars[main] = true;
                }
            }
            return uniqueChars.Keys.ToList();
        }
    }
}
