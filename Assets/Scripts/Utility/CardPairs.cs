using System.Collections.Generic;

namespace Utility {
    public static class CardPairs {

        private static Dictionary<char, char> pairLookup = new();

        public static char GetPair(char character) {
            CheckPairLookup();
            var c = character.ToString().ToUpper()[0];
            if (!pairLookup.ContainsKey(c)) {
                return '#';
            }
            return pairLookup[c];
        }

        private static void CheckPairLookup() {
            if (pairLookup.Count > 0) {
                return;
            }

            pairLookup['A'] = 'E';
            pairLookup['B'] = 'Q';
            pairLookup['C'] = 'O';
            pairLookup['D'] = 'P';
            pairLookup['E'] = 'A';
            pairLookup['F'] = 'T';
            pairLookup['G'] = 'K';
            pairLookup['H'] = 'Y';
            pairLookup['I'] = 'L';
            pairLookup['J'] = 'R';
            pairLookup['K'] = 'G';
            pairLookup['L'] = 'I';
            pairLookup['M'] = 'W';
            pairLookup['N'] = 'U';
            pairLookup['O'] = 'C';
            pairLookup['P'] = 'D';
            pairLookup['Q'] = 'B';
            pairLookup['R'] = 'J';
            pairLookup['S'] = 'V';
            pairLookup['T'] = 'F';
            pairLookup['U'] = 'N';
            pairLookup['V'] = 'S';
            pairLookup['W'] = 'M';
            pairLookup['X'] = 'Z';
            pairLookup['Y'] = 'H';
            pairLookup['Z'] = 'X';
        }
    }
}
