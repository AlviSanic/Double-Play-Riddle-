using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Riddle {
    public static class RiddleProvider {

        private static readonly (string answer, string riddle)[] riddles = new (string, string)[] {
            ("clock", "I have hands but never clap or wave.\nI keep the time, your minutes save."),
            ("river","I move without legs, run very fast.\nYet I stay still as I rush past."),
            ("cloud","I'm white or gray and float with air.\nI'm soft to see but vanish there."),
            ("voice","I go through walls but leave no trace.\nYou hear me talk in every place."),
            ("chalk","I chase the sun and flee the rain.\nI draw on paths, I leave a stain."),
            ("towel","I dry you off but start out wet.\nI'm soft and warm, your cleanest bet."),
            ("water","I'm clear and clean and full of splash.\nI fill your glass or bathtub dash."),
            ("music","I count the beat and help you move.\nI play the songs with catchy groove."),
            ("watch","I'm worn for time but never old.\nI'm round and fast, I shine like gold."),
            ("earth","I spin and spin without a top.\nI never sleep, I never stop."),
            ("chips","I crunch and crack with every bite.\nI'm golden brown and bring delight."),
            ("shoes","I walk in pairs but never speak.\nI climb the stairs but never squeak."),
            ("socks","I hide your feet and match your shoe.\nI come in pairs and work for you."),
            ("smoke","I hang around and block the sun.\nI form from fire and ruin fun."),
            ("ruler","I'm used in math to measure size.\nI help you draw straight and wise."),
            ("start","I mark the time when things begin.\nI help you race and sometimes win."),
            ("steam","I rise and puff but have no smoke.\nI twist and swirl, a drifting cloak."),
            ("stars","I sparkle bright and twinkle small.\nI fill the sky and never fall."),
            ("scarf","I hide the cold, I block the breeze.\nI hug your neck in winter freeze."),
            ("mouth","I open wide to shout or sing.\nI eat and taste most everything."),
            ("train","I puff and steam, I ride the track.\nI toot and go, then hurry back."),
            ("apple","I'm red or green, I crunch and pop.\nA fruit you grab right from the top."),
            ("brain","I help you think and plan ahead.\nI'm in your skull, not made of bread."),
            ("light","I brighten rooms when it's too dark.\nI shine at night and leave a mark."),
            ("flame","I'm hot and bright, I dance with fire.\nI burn up wood, I never tire."),
            ("bread","You slice me fresh or toast me hot.\nI'm soft and brown in every slot."),
            ("beach","I touch the shore and kiss your toes.\nWith sun and sand, I come and go."),
            ("chair","I help you rest your legs all day.\nI have four legs but never stray."),
            ("glove","I cover hands from cold and snow.\nWith fingers tucked, I help you go."),
            ("plant","I grow in dirt and stretch up high.\nWith leaves and stems, I reach the sky."),
            ("brush","You use me daily, stroke or scrub.\nI clean your teeth or paint the tub."),
            ("crane","I lift things high and fly with might.\nI'm found near docks or worksite light."),
            ("stone","I'm hard and gray and found on trails.\nYou skip me flat or weigh your scales."),
            ("trace","I show the marks you leave behind.\nI help detectives when they find."),
            ("shelf","I hold your books and things in place.\nI'm nailed to walls or sit in space."),
            ("flute","I'm played with wind and fingers light.\nI make a sound both clear and bright."),
            ("swing","I hang and sway in parks with glee.\nI lift you high, then set you free."),
            ("table","You eat on me or do your work.\nI stand on legs, I never shirk."),
            ("track","I lead your feet and help your pace.\nOn running path, I set the race."),
            ("drill","I spin and twist and make a hole.\nI dig through wood and take a toll."),
            ("spine","I hold your back and bones inside.\nI help you stand and stretch with pride."),
            ("stool","You sit on me when chairs are full.\nI'm round or tall and sometimes dull."),
            ("tiger","I'm striped and fast, I like to growl.\nI'm big and wild, I leap and prowl."),
            ("spoon","I stir your soup or help you eat.\nI carry food that's soft or sweet."),
            ("eagle","I soar up high with sharpest sight.\nI glide through clouds in silent flight."),
            ("flash","I shine real fast, a sudden spark.\nI light the sky then fade to dark."),
            ("glass","I'm clear and break with sudden force.\nI'm used for cups and windows, of course."),
            ("crack","I split the wall and make loud noise.\nI happen fast, I break your voice."),
            ("knife","I slice through food and help you prep.\nI'm very sharp and must be kept."),
            ("wheel","I roll and spin but never sleep.\nI'm on your bike or when cars leap."),
            ("broom","I sweep the dust from every room.\nI clean the floor and chase the gloom."),
            ("match","I burn and match, I start the flame.\nI help you light a candle's name."),
            ("drums","I pound a beat with sticks or hands.\nI'm part of music-playing bands."),
            ("frame","I hold your art or photo still.\nI hang on walls or windowsill."),
            ("torch","I shine ahead when night is black.\nI help you walk the safest track."),
            ("frost","I'm white and cold and form on glass.\nI chill the air and crunch the grass."),
            ("lemon","I'm yellow, sour, full of zest.\nI flavor tea and cook the best."),
            ("piano","I have black keys and ivory white.\nI make soft music, day or night."),
            ("shark","I swim with speed and rows of teeth.\nI'm feared below and fast beneath."),
            ("horse","I run through fields and sleep in hay.\nI gallop fast, I neigh all day."),
            ("paper","I crinkle up and fold with ease.\nI hold your notes and catch a breeze."),
            ("waves","I hit the shore and make a splash.\nI curl and foam and break with crash."),
            ("sting","I jab and buzz and leave a welt.\nMy sting is quick, it can be felt."),
            ("drake","I swim and quack and flap with pride.\nI glide on ponds and love to slide."),
            ("skate","I glide on ice with wheels or blades.\nI turn and spin in thrilling parades."),

            /*

            ("Vapor", "You see me briefly, feel me vanish. What am I?"),
            ("Whirl", "I spin and twirl like wind. What am I?"),
            ("Crack", "I split but don't destroy. What am I?"),
            ("Glint", "I catch the eye with flash. What am I?"),
            ("Beast", "I roar in myths and jungles. What am I?"),
            ("Chill", "I crawl down spines and open windows. What am I?"),
            ("Spine", "I hold you up and twist. What am I?"),
            ("Bloom", "I open with sun and close by dusk. What am I?"),
            ("Quirk", "A trait odd but lovable. What am I?"),
            ("Shade", "I follow light and cool ground. What am I?"),
            ("Creak", "I groan when moved, old and wood. What am I?"),
            ("Snarl", "A twisted knot or beast's warning. What am I?"),
            ("Truth", "I stand even when lies flood. What am I?"),
            ("Feast", "I gather plates, joy, and hunger. What am I?"),
            ("Ridge", "A back of earth or spine. What am I?"),
            ("Glide", "I move smooth with no feet. What am I?"),
            ("Grime", "I cling to corners and hands. What am I?"),
            ("Piano", "I sing when struck in silence. What am I?"),
            ("Blaze", "I devour with light and heat. What am I?"),
            ("Spore", "Tiny traveler, I start growth. What am I?"),
            ("Gloom", "I dim the day with thought. What am I?"),
            ("Shove", "I move you without a word. What am I?"),
            ("Latch", "I hold shut with quiet click. What am I?"),
            ("Chirp", "Small and cheerful, I sing mornings. What am I?"),
            ("Brisk", "I'm cool, quick, and morning air. What am I?"),
            ("Grail", "I'm sought by knights and dreams. What am I?"),
            ("Crust", "I'm baked, tough, and outermost. What am I?"),
            ("Drake", "A dragon's cousin, sometimes bird. What am I?"),
            ("Wreck", "I used to be whole. What am I?"),
            ("Fable", "I tell truth through lies. What am I?"),
            ("Hinge", "I let things turn but never move far. What am I?"),
            ("Sleet", "I fall cold and sting skin. What am I?"),
            ("Sting", "Small but sharp, I teach pain. What am I?"),
            ("Moist", "I'm not dry but not wet. What am I?"),
            ("Hoist", "I lift with force or rope. What am I?"),
            ("Slope", "I rise or fall with angle. What am I?"),
            ("Trace", "I'm left behind or drawn. What am I?"),
            ("Crook", "A thief or bend in staff. What am I?"),
            ("Glare", "I shine harsh or stare hard. What am I?"),
            ("Cliff", "Stand tall and fall follows. What am I?"),
            ("Brute", "I move without manners or thought. What am I?"),
            ("Roast", "I'm hot, seasoned, and Sunday's pride. What am I?"),
            ("Swoop", "I dive fast from above. What am I?"),
            ("Scoop", "I lift small things in curves. What am I?")
            */
        };

        private static readonly DateTime startDate = new DateTime(2025, 12, 06);

        private static IEnumerator LoadRiddle(Action<string> jsonCallback) {
            using (var req = UnityWebRequest.Get("/api/riddle")) {
                yield return req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.Success) {
                    var json = req.downloadHandler.text;
                    jsonCallback?.Invoke(json);
                }
                else {
                    jsonCallback?.Invoke("{ \"answer\": \"Error\", \"riddle\": \"Error\" }");
                }
            }
        }

        public class RiddleItem {
            public string answer;
            public string riddle;
            public int riddleId;
        }

        public static IEnumerator GetRiddleOfTheDay(int debugDays, Action<RiddleItem> onFinished) {

            yield return LoadRiddle((json) => {
                Debug.Log("JSON RETURN: " + json);
                var data = JsonUtility.FromJson<RiddleItem>(json);
                onFinished?.Invoke(new RiddleItem() { answer = data.answer, riddle = data.riddle, riddleId = data.riddleId });
            });

            // var today = DateTime.Now.AddDays(debugDays);
            // var daysDifference = GetDaysDifference(today);
            // if (daysDifference >= riddles.Length) {
            //     index = -1;
            //     return ("play", "No More Riddle");
            // }
            // index = daysDifference;
            // return riddles[index];
        }

        private static int GetDaysDifference(DateTime dateTime) {
            var currentDate = dateTime;
            var daysDifference = (int)(currentDate - startDate).TotalDays;
            return daysDifference;
        }
    }
}
