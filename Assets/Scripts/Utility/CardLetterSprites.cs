using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    [CreateAssetMenu(fileName = "CardLetterSprites", menuName = "Game/CardLetterSprites")]
    public class CardLetterSprites : ScriptableObject {

        private static CardLetterSprites instance;

        public static CardLetterSprites Instance {
            get {
                if (instance == null) {
                    instance = Resources.Load<CardLetterSprites>("CardLetterSprites");
                    if (instance == null) {
                        Debug.LogError("CardLetterSprites instance not found in Resources!");
                    }
                }
                return instance;
            }
        }


        [SerializeField]
        private Sprite a;
        [SerializeField]
        private Sprite b;
        [SerializeField]
        private Sprite c;
        [SerializeField]
        private Sprite d;
        [SerializeField]
        private Sprite e;
        [SerializeField]
        private Sprite f;
        [SerializeField]
        private Sprite g;
        [SerializeField]
        private Sprite h;
        [SerializeField]
        private Sprite i;
        [SerializeField]
        private Sprite j;
        [SerializeField]
        private Sprite k;
        [SerializeField]
        private Sprite l;
        [SerializeField]
        private Sprite m;
        [SerializeField]
        private Sprite n;
        [SerializeField]
        private Sprite o;
        [SerializeField]
        private Sprite p;
        [SerializeField]
        private Sprite q;
        [SerializeField]
        private Sprite r;
        [SerializeField]
        private Sprite s;
        [SerializeField]
        private Sprite t;
        [SerializeField]
        private Sprite u;
        [SerializeField]
        private Sprite v;
        [SerializeField]
        private Sprite w;
        [SerializeField]
        private Sprite x;
        [SerializeField]
        private Sprite y;
        [SerializeField]
        private Sprite z;

        private Dictionary<char, Sprite> spriteLookup = new();

        public Sprite GetSprite(char character) {
            CheckSpriteLookup();
            var c = character.ToString().ToUpper()[0];
            if (!spriteLookup.ContainsKey(c)) {
                return null;
            }
            return spriteLookup[c];
        }

        private void Start() {
            CheckSpriteLookup();
        }

        private void CheckSpriteLookup() {
            if (spriteLookup.Count > 0) {
                return;
            }

            spriteLookup['A'] = a;
            spriteLookup['B'] = b;
            spriteLookup['C'] = c;
            spriteLookup['D'] = d;
            spriteLookup['E'] = e;
            spriteLookup['F'] = f;
            spriteLookup['G'] = g;
            spriteLookup['H'] = h;
            spriteLookup['I'] = i;
            spriteLookup['J'] = j;
            spriteLookup['K'] = k;
            spriteLookup['L'] = l;
            spriteLookup['M'] = m;
            spriteLookup['N'] = n;
            spriteLookup['O'] = o;
            spriteLookup['P'] = p;
            spriteLookup['Q'] = q;
            spriteLookup['R'] = r;
            spriteLookup['S'] = s;
            spriteLookup['T'] = t;
            spriteLookup['U'] = u;
            spriteLookup['V'] = v;
            spriteLookup['W'] = w;
            spriteLookup['X'] = x;
            spriteLookup['Y'] = y;
            spriteLookup['Z'] = z;
        }
    }
}
