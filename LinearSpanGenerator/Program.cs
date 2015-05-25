using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LinearSpanGenerator
{
    class Program
    {
        internal class Word : List<Pair>
        {
            protected bool Equals(Word other)
            {
                if (other == null)
                    return false;

                if (other.Count != this.Count)
                    return false;

                for (int i = 0; i < Count; i++)
                {
                    if (!Equals(this[i], other[i]))
                        return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                return this.ToString().GetHashCode();
            }

            internal Word(string str)
            {
                var length = str.Length;

                for (int i = 0; i < length - 1; i += 2)
                {
                    this.Add(new Pair(str[i], str[i+1]));
                }
            }

            public override string ToString()
            {
                return this.Aggregate(string.Empty, (current, pair) => current + pair);
            }

            public override bool Equals(object obj)
            {
                var other = obj as Word;
                if (other == null)
                    return false;

                if (other.Count != this.Count)
                    return false;

                for (int i = 0; i < Count; i++)
                {
                    if (!Equals(this[i], other[i]))
                        return false;
                }

                return true;
            }

            public static Word operator +(Word f, Word s)
            {
                var result = new Word("");

                if (f.Count != s.Count)
                    return result;

                result.AddRange(f.Select((t, i) => t + s[i]));

                return result;
            }
        }

        internal struct Pair
        {
            internal Pair(short n1, short n2) : this()
            {
                N1 = n1;
                N2 = n2;
            }

            internal Pair(char n1, char n2) : this()
            {
                N1 = short.Parse(n1.ToString());
                N2 = short.Parse(n2.ToString());
            }

            internal Pair(string str) : this()
            {
                N1 = short.Parse(str[0].ToString());
                N2 = short.Parse(str[1].ToString());
            }

            public short N1 { get; set; }
            public short N2 { get; set; }

            public static Pair operator +(Pair f, Pair s)
            {
                return Calculator.GetResult(f, s);
            }

            public override string ToString()
            {
                return N1.ToString() + N2.ToString();
            }

            public bool Is(short n1, short n2)
            {
                return N1 == n1 && N2 == n2;
            }
        }

        internal static class Calculator
        {
            internal static Pair GetResult(Pair f, Pair s)
            {
                if ((f.Is(0, 0) && s.Is(0, 0)) ||
                    (f.Is(0, 1) && s.Is(1, 0)) ||
                    (f.Is(1, 0) && s.Is(0, 1)) ||
                    (f.Is(1, 1) && s.Is(1, 1)))
                    return new Pair(0, 0);
                else if ((f.Is(0, 0) && s.Is(0, 1)) ||
                         (f.Is(0, 1) && s.Is(0, 0)) ||
                         (f.Is(1, 0) && s.Is(1, 1)) ||
                         (f.Is(1, 1) && s.Is(1, 0)))
                    return new Pair(0, 1);
                else if ((f.Is(0, 0) && s.Is(1, 0)) ||
                         (f.Is(0, 1) && s.Is(1, 1)) ||
                         (f.Is(1, 0) && s.Is(0, 0)) ||
                         (f.Is(1, 1) && s.Is(0, 1)))
                    return new Pair(1, 0);
                else
                    return new Pair(1, 1);
            }
        }

        static void Main(string[] args)
        {
            var path = "input.txt";

            if (args.Any())
                path = args[0];

            var result = new List<Word>();
            ICollection<string> text;

            using (var f = File.OpenText(path))
            {
                text = f.ReadToEnd().Split(' ', '\n');
            }

            var zeroStr = text.FirstOrDefault(s => s.StartsWith("#"));
            ICollection<int> zeros = null;
            if (zeroStr != null)
            {
                try
                {
                    zeros = zeroStr.Replace("#", "").Split(' ').Select(s => int.Parse(s)).ToList();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            
            var numbers = text.Where(s => !s.StartsWith("#")).ToList();

            result.AddRange(numbers.Select(s => new Word(s)));
            result = result.Distinct().ToList();

            var unsummed = result.ToList();
            var summed = new List<Word>();

            while (unsummed.Any())
            {
                var w = unsummed.First();

                var news = result.Select(r => w + r).ToList();

                unsummed.Remove(w);
                summed.Add(w);
                unsummed.AddRange(news.Where(n => !summed.Contains(n)));
                unsummed = unsummed.Distinct().ToList();
                result.AddRange(news);
                result = result.Distinct().ToList();
            }

            var ts = result.Select(r => r.ToString()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            ts.Sort();

            var suitable = ts.Count(s => zeros == null || !zeros.Any() || zeros.All(i => s.Length < i || s[i - 1] == '0'));

            StringBuilder str = new StringBuilder();
            str.AppendLine(suitable.ToString());
            foreach (var t in ts)
            {
                str.AppendLine(t);
            }

            File.WriteAllText("output.txt", str.ToString());
        }
    }
}
