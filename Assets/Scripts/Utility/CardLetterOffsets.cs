using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    public static class CardLetterOffsets {

        private static Dictionary<char, (Vector2 position, float rotation)> offsetLookup = new();

        public static (Vector2 position, float rotation) GetOffset(char character) {
            CheckOffsetLookup();
            var c = character.ToString().ToUpper()[0];
            if (!offsetLookup.ContainsKey(c)) {
                return (Vector2.zero, 0f);
            }
            return offsetLookup[c];
        }

        private static void CheckOffsetLookup() {
            if (offsetLookup.Count > 0) {
                return;
            }

            offsetLookup['A'] = (Vector2.zero, 0f);
            offsetLookup['B'] = (Vector2.zero, 180f);
            offsetLookup['C'] = (Vector2.zero, 180f);
            offsetLookup['D'] = (Vector2.zero, 0f);
            offsetLookup['E'] = (Vector2.zero, 180f);
            offsetLookup['F'] = (new Vector2(0f, 0.25f * 0.28f), 180f);
            offsetLookup['G'] = (Vector2.zero, 0f);
            offsetLookup['H'] = (Vector2.zero, 180f);
            offsetLookup['I'] = (new Vector2(0f, 0.1f * 0.28f), 0f);
            offsetLookup['J'] = (new Vector2(0f, -0.15f * 0.28f), 0f);
            offsetLookup['K'] = (new Vector2(0f, -0.1f * 0.28f), 180f);
            offsetLookup['L'] = (new Vector2(0f, 0.1f * 0.28f), 180f);
            offsetLookup['M'] = (Vector2.zero, 0f);
            offsetLookup['N'] = (Vector2.zero, 180f);
            offsetLookup['O'] = (Vector2.zero, 0f);
            offsetLookup['P'] = (Vector2.zero, 180f);
            offsetLookup['Q'] = (Vector2.zero, 0f);
            offsetLookup['R'] = (new Vector2(0f, 0f), 180f);
            offsetLookup['S'] = (Vector2.zero, 0f);
            offsetLookup['T'] = (Vector2.zero, 0f);
            offsetLookup['U'] = (Vector2.zero, 0f);
            offsetLookup['V'] = (Vector2.zero, 180f);
            offsetLookup['W'] = (Vector2.zero, 180f);
            offsetLookup['X'] = (Vector2.zero, 180f);
            offsetLookup['Y'] = (Vector2.zero, 0f);
            offsetLookup['Z'] = (Vector2.zero, 0f);
        }
    }
}
