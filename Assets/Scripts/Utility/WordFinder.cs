using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    public class WordFinder {

        public static WordFinder Instance { private set; get; } = null;
        public static int MaxWordsFoundFromLetters => Instance.maxWordsFoundFromLetters;
        public static int MaxLettersInAWord => Instance.maxLettersInAWord;
        public static int MinLetterCount => Instance.minLetterCount;

        public static string[] FindWords(string jumbled, int numLetters = -1) => Instance.FindWordsInternal(jumbled, numLetters);

        public static void CreateInstance(string dictionaryContent) {
            if (Instance != null) {
                //return;
            }
            Instance = new WordFinder(dictionaryContent);
        }

        public readonly int maxWordsFoundFromLetters = 40;
        public readonly int maxLettersInAWord = 8;
        public readonly int minLetterCount = 2;

        // LetterCount, (Word, List of Sorted Letters))
        private Dictionary<int, Dictionary<string, string>> _database = new();

        private WordFinder(string dictionaryContent) {
            LoadDatabase(dictionaryContent);
        }

        public void LoadDatabase(string dictionaryContent) {
            _database?.Clear();
            var separators = new char[] { '\n', '\r' };
            var words = dictionaryContent.Split(separators);
            for (int i = 0; i < words.Length; i++) {
                AddWord(words[i]);
            }
        }

        public async void HasWordAsync(string jumbled, int numLetters, Action<bool> onReturn) {
            var result = await Task.Run<bool>(() => {
                return HasWord(jumbled, numLetters);
            });
            onReturn?.Invoke(result);
        }

        public bool IsWordValid(string word) {
            word = word.ToLower();
            return _database.ContainsKey(word.Length) && _database[word.Length].ContainsKey(word);
        }

        public bool FindWord(string word, string jumbled) {
            word = word.ToLower();
            jumbled = jumbled.ToLower();
            var words = FindWordsInternal(jumbled, word.Length);
            foreach (string w in words) {
                if (w.Equals(word)) {
                    return true;
                }
            }
            return false;
        }

        public bool HasWord(string jumbled, int numLetters = -1) {
            var word = FindWords(jumbled, numLetters, "", 1);
            return (word != null && word.Length > 0);
        }

        public async void FindWordsAsync(string jumbled, int numLetters, Action<string[]> onReturn) {
            var result = await Task.Run<string[]>(() => {
                return FindWords(jumbled, numLetters, "", maxWordsFoundFromLetters);
            });
            onReturn?.Invoke(result);
        }

        public string[] FindWordsInternal(string jumbled, int numLetters = -1) {
            return FindWords(jumbled, numLetters, "", maxWordsFoundFromLetters);
        }

        public string[] FindWords(string jumbled, int numLetters, string containsLetters, int maxWordCount) {
            if (_database == null) {
                return null;
            }

            jumbled = jumbled.ToLower();
            var allLetters = jumbled + containsLetters;
            var words = new List<string>();

            FindAllPossibleWords(allLetters, containsLetters, words, numLetters, maxWordCount);

            string[] w = new string[words.Count];
            for (int i = 0; i < words.Count; i++) {
                w[i] = words[i];
            }
            return w;
        }

        public void AddWord(string word) {
            if (word.Equals("")) {
                return;
            }

            var wordLength = word.Length;
            Dictionary<string, string> map = null;

            if (_database.ContainsKey(wordLength)) {
                map = _database[wordLength];
            }
            else {
                map = new Dictionary<string, string>();
                _database.Add(wordLength, map);
            }

            if (!map.ContainsKey(word)) {
                map.Add(word, GetSortedLetterOfWord(word));
            }
        }

        public string[] FindDoublePlayWords(string jumbled, int numLetters, string containsLetters, int maxWordCount) {
            var words = new List<string>();
            var permutations = new List<string>();
            permutations.AddRange(FindDoublePlayPermutations(jumbled));
            var permutationsStr = "";
            foreach (var p in permutations) {
                permutationsStr += p + ", ";
                var foundWords = new List<string>(FindWords(p, numLetters));
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

        public int ComputeScrabbleScore(string word) {
            var total = 0;
            var charArray = word.ToCharArray();
            foreach (char c in charArray) {
                switch (c) {
                    case 'a':
                        total += 1;
                        break;
                    case 'b':
                        total += 3;
                        break;
                    case 'c':
                        total += 3;
                        break;
                    case 'd':
                        total += 2;
                        break;
                    case 'e':
                        total += 1;
                        break;
                    case 'f':
                        total += 4;
                        break;
                    case 'g':
                        total += 2;
                        break;
                    case 'h':
                        total += 4;
                        break;
                    case 'i':
                        total += 1;
                        break;
                    case 'j':
                        total += 8;
                        break;
                    case 'k':
                        total += 5;
                        break;
                    case 'l':
                        total += 1;
                        break;
                    case 'm':
                        total += 3;
                        break;
                    case 'n':
                        total += 1;
                        break;
                    case 'o':
                        total += 1;
                        break;
                    case 'p':
                        total += 3;
                        break;
                    case 'q':
                        total += 10;
                        break;
                    case 'r':
                        total += 1;
                        break;
                    case 's':
                        total += 1;
                        break;
                    case 't':
                        total += 1;
                        break;
                    case 'u':
                        total += 1;
                        break;
                    case 'v':
                        total += 4;
                        break;
                    case 'w':
                        total += 4;
                        break;
                    case 'x':
                        total += 8;
                        break;
                    case 'y':
                        total += 4;
                        break;
                    case 'z':
                        total += 10;
                        break;
                }
            }

            return total;
        }

        private string GetSortedLetterOfWord(string word) {
            var cs = word.ToCharArray();
            Array.Sort(cs);
            return new string(cs);
        }

        private void FindAllPossibleWords(string jumbled, string containLetters, List<string> result, int numLetters, int maxResultWords) {
            var testers = new Dictionary<char, int>();
            var jumbledChars = new char[jumbled.Length];
            jumbledChars = jumbled.ToCharArray();
            Array.Sort(jumbledChars);

            var sortedJumbled = new string(jumbledChars);
            var maxLetterCount = jumbled.Length;
            maxLetterCount = maxLetterCount > maxLettersInAWord ? maxLettersInAWord : maxLetterCount;

            var loopEnder = minLetterCount;
            if (numLetters != -1) {
                maxLetterCount = numLetters;
                loopEnder = numLetters;
            }

            for (int i = maxLetterCount; i >= loopEnder; i--) {
                if (!_database.ContainsKey(i)) {
                    continue;
                }

                var words = _database[i];
                if (words == null) {
                    continue;
                }

                foreach (KeyValuePair<string, string> pair in words) {
                    bool match = true;
                    string sortedWord = pair.Value;
                    testers.Clear();

                    for (int i2 = 0; i2 < sortedJumbled.Length; i2++) {
                        char c = sortedJumbled[i2];
                        if (testers.ContainsKey(c)) {
                            int integer = testers[c];
                            integer++;
                            testers[c] = integer;
                        }
                        else {
                            testers.Add(c, 1);
                        }
                    }

                    for (int i2 = 0; i2 < sortedWord.Length; i2++) {
                        char c = sortedWord[i2];
                        if (testers.ContainsKey(c)) {
                            int integer = testers[c];
                            integer--;

                            if (integer >= 0) {
                                testers[c] = integer;
                            }
                            else {
                                //Lacks letter duplication
                                match = false;
                                break;
                            }
                        }
                        else {
                            //Not a match
                            match = false;
                            break;
                        }
                    }

                    if (!match) {
                        continue;
                    }

                    //Proceed here if it's a match; save the word for display
                    if (maxResultWords > 0) {
                        if (pair.Key.Contains(containLetters)) {
                            result.Add(pair.Key);
                            maxResultWords--;
                        }
                    }
                    else {
                        if (pair.Key.Contains(containLetters)) {
                            result.Add(pair.Key);
                        }
                    }
                }
            }
        }

        // ------------------------------------------------------------------------------------
        // -- FindCombinations and FindAllCombinations is not working as intended.
        // ------------------------------------------------------------------------------------
        private void FindCombinations(string jumbled, int offset, int maxLetters, String temp, Dictionary<string, bool> result) {
            if (maxLetters == 0) {
                var str = temp;
                if (str.Length > maxLettersInAWord) {
                    return;
                }

                var charArray = str.ToCharArray();
                Array.Sort(charArray);

                var charArrayStr = new string(charArray);
                if (!result.ContainsKey(charArrayStr)) {
                    result.Add(charArrayStr, true);
                }
                return;
            }

            for (int i = offset; i <= (int)jumbled.Length - maxLetters; i++) {
                int tempLastIndex = temp.Length;
                temp += jumbled[i];
                FindCombinations(jumbled, i + 1, maxLetters - 1, temp, result);
                temp.Remove(tempLastIndex, 1);
            }
        }

        public void FindAllCombinations(string jumbled, List<string> result) {
            for (int i = (int)jumbled.Length; i >= 2; i--) {
                var allCombinations = new Dictionary<string, bool>();
                String temp = "";
                FindCombinations(jumbled, 0, i, temp, allCombinations);
                foreach (var pair in allCombinations) {
                    result.Add(pair.Key);
                }
            }
        }
    }
}
