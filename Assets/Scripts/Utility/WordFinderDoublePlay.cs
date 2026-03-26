using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
namespace Utility {
    public class WordFinderDoublePlay {

        public async void HasDoublePlayWordAsync(string jumbled, int numLetters, Action<bool> onReturn) {
            var result = await Task.Run<bool>(() => {
                return HasDoublePlayWord(jumbled, numLetters);
            });
            onReturn?.Invoke(result);
        }

        public bool HasDoublePlayWord(string jumbled, int numLetters = -1) {
            var word = FindDoublePlayWords(jumbled, numLetters, "", 1);
            return (word != null && word.Length > 0);
        }

        public async void FindDoublePlayWordsAsync(string jumbled, int numLetters, Action<string[]> onReturn) {
            var result = await Task.Run<string[]>(() => {
                return FindDoublePlayWords(jumbled, numLetters, "", WordFinder.MaxWordsFoundFromLetters);
            });
            onReturn?.Invoke(result);
        }

        public string[] FindDoublePlayWords(string jumbled, int numLetters = -1) {
            return FindDoublePlayWords(jumbled, numLetters, "", WordFinder.MaxWordsFoundFromLetters);
        }

        public string[] FindDoublePlayWords(string jumbled, int numLetters, string containsLetters, int maxWordCount) {
            var words = new List<string>();
            var permutations = new List<string>();
            permutations.AddRange(FindDoublePlayPermutations(jumbled));
            var permutationsStr = "";
            foreach (var p in permutations) {
                permutationsStr += p + ", ";
                var foundWords = new List<string>(WordFinder.FindWords(p, numLetters));
                foundWords.RemoveAll(item => words.Contains(item));
                words.AddRange(foundWords);
            }
            return words.ToArray();
        }

        public string[] FindDoublePlayPermutations(string jumbled) {
            var permutations = new List<string>();
            var count = Mathf.Pow(2, jumbled.Length);
            //Debug.Log("DoublePlay Permutations for " + jumbled.Length + " letter word is: " + count);

            for (var i = 0; i < count; i++) {
                var flags = Convert.ToString(i, 2).PadLeft(jumbled.Length, '0');
                var permutation = "";
                for (var i2 = 0; i2 < jumbled.Length; i2++) {
                    permutation += flags[i2].Equals('0') ? jumbled[i2] : CardPairs.GetPair(jumbled[i2]);
                }
                permutations.Add(permutation.ToUpper());
            }
            return permutations.ToArray();
        }
    }
}
