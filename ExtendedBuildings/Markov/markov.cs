using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Math;

namespace ExtendedBuildings
{
    public class Markov
    {
        private Dictionary<string, Ngram> pairs = new Dictionary<string, Ngram>();
        private Dictionary<string, int> starters = new Dictionary<string, int>();
        readonly int n;
        readonly bool isWord;

        public Markov(string resourceName, bool useWords, int n)
        {
            var buffer = Localization.GetResource(resourceName);           
            this.n = n;
            isWord = useWords;
            for (int i = 0; i < buffer.Count; i++)
                if (buffer[i].Length > n)
                {
                    if (useWords)
                        GenerateByWord(buffer[i].Trim());
                    else
                        GenerateByChar(buffer[i].Trim());
                }
        }

        private void GenerateByWord(string p)
        {
            var line = p.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (line.Length < n)
                return;

            var result = new string[n];
            for (var i = 0; i < line.Length - n; i += 1)
            {
                for (var j = 0; j < n; j += 1)
                    result[j] = line[i + j];

                var rr = String.Join(" ", result);
                if (i == 0)
                {
                    if (!starters.ContainsKey(rr))
                        starters.Add(rr, 0);

                    starters[rr] += 1;
                }

                if (!pairs.ContainsKey(rr))
                    pairs.Add(rr, new Ngram());

                pairs[rr].Add(i + n < line.Length ? line[i + n] : "");
            }
        }

        private void GenerateByChar(string p)
        {
            var line = p.ToCharArray();
            if (line.Length < n)
                return;

            for (var i = 0; i < line.Length - n; i += 1)
            {
                var result = "";
                for (var j = 0; j < n; j += 1)
                    result += line[i + j];

                if (i == 0)
                {
                    if (!starters.ContainsKey(result))
                        starters.Add(result, 0);

                    starters[result] += 1;
                }

                if (!pairs.ContainsKey(result))
                    pairs.Add(result, new Ngram());

                pairs[result].Add(i + n < line.Length ? new String(line, i + n, 1) : "");
            }
        }

        public string GetText(int min, int max, bool useStarter)
        {
            var rand = new Randomizer(DateTime.Now.Ticks);
            return GetText(ref rand, min, max, useStarter);
        }

        public string GetText(ref Randomizer rand, int min, int max, bool useStarter, bool endOnSpaces = false)
        {
            var what = rand.Int32(41u);
            var result = new List<string>();
            
            while (result.Count() < min)
            {
                var starter = useStarter ? GetRandomStarter(ref rand) : pairs.Keys.ElementAt(rand.Int32((uint)pairs.Count()));
                var current = pairs[starter];

                result.AddRange(isWord ? starter.Split(' ') : starter.ToCharArray().Select(cc => cc.ToString()));

                for (var i = 0; i < 1000; i += 1)
                {
                    var next = current.GetNext(ref rand);
                    if (next == "")
                        break;

                    result.Add(next);

                    var count = result.Count() - n;
                    var nextCurrent = new string[n];
                    for (var j = 0; j < n; j += 1)
                        nextCurrent[j] = result[count + j];

                    var nc = String.Join(isWord ? " " : "", nextCurrent);

                    if (!pairs.ContainsKey(nc))
                        break;

                    current = pairs[nc];
                    var lastWord = result[result.Count - 1];
                    var lastChar = lastWord.Substring(lastWord.Length - 1);
                    if (result.Count > ((max - min) / 2 + min))
                    {
                        var randValue = rand.Int32((uint)(Math.Max(0,max * 2 - result.Count)));
                        if (endOnSpaces && (lastChar == " " && randValue < 3)
                            || (lastChar == "."|| lastChar == "!") && randValue < 8)
                            break;
                    }
                    
                }

                if (!isWord)
                    result.Add(" ");
            }

            return string.Join(isWord ? " " : "", result.ToArray()).Trim();
        }

        private string GetRandomStarter(ref Randomizer rand)
        {
            var total = starters.Sum(c => c.Value);
            int target = rand.Int32((uint)total);
            target = rand.Int32((uint)total);
            int sum = 0;
            foreach (var kvp in starters)
            {
                sum += kvp.Value;
                if (sum >= target)
                    return kvp.Key;
            }
            return null;
        }

    }

    class Ngram
    {
        public List<string> Following = new List<string>();
        
        internal void Add(string p) => Following.Add(p);

        public string GetNext(ref Randomizer rand) => Following[rand.Int32((uint)Following.Count)];
    }
}
