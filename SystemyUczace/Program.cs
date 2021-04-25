using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SystemyUczace
{
    class Program
    {

        public static bool debug = false;
        
        public static Node Tree;
        static string spliter;
        static string[][] data;
        public static void log(string text, int method = 1, int force = 0)
        {
            if ((debug && method == 1) || (force == 1))
                Console.WriteLine(text);
            else if ((debug && method == 2) || (force == 1))
                Console.Write(text);
        }
        public static void ShowTable(string[][] array,int force = 0)
        {
            foreach (string[] row in array)
            {
                foreach (string value in row)
                {
                    Console.Write(value);
                    Console.Write(" ");
                }
                Console.WriteLine("");
            }
        }
        public static double Entropy(IDictionary dec)
        {
            double EntropyValue = 0;
            double count = 0;
            List<int> items = new List<int>();
            foreach (DictionaryEntry de in dec)
            {
                items.Add(Convert.ToInt32(de.Value));
                count += Convert.ToInt32(de.Value);
            }
            foreach (int i in items)
            {
                var p = i / count;
                EntropyValue += p * Math.Log2(p);
            }
            return -1 * EntropyValue;
        }
        // Info(a1,T) = Tnew/T * Info(New)
        public static double Info(string[] data, IDictionary dic, string[] des)
        {
            double sum = 0;

            foreach (DictionaryEntry de in dic)
            {
                double x = 0;
                IDictionary<string, int> count = new Dictionary<string, int>();
                x = (Convert.ToSingle(de.Value) / data.Length);
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == Convert.ToString(de.Key))
                    {
                        if (count.ContainsKey(des[i]))
                        {
                            count[des[i]] = count[des[i]] + 1;
                        }
                        else
                        {
                            count.Add(des[i], 1);
                        }
                    }
                }
                sum += (x * Entropy((IDictionary)count));
            }
            return sum;
        }
        public static List<Dictionary<string, int>> GetAppearList(string[][] data)
        {
            List<Dictionary<string, int>> appearList = new List<Dictionary<string, int>>();
            for (int i = 0; i < data[0].Length; i++)
            {
                string[] kolumna = new string[data.Length];
                IDictionary<string, int> countDictionary = new Dictionary<string, int>();
                for (int j = 0; j < data.Length; j++)
                {
                    var tmp = data[j][i];
                    kolumna[j] = tmp;
                    if (countDictionary.ContainsKey(tmp))
                    {
                        countDictionary[tmp] = countDictionary[tmp] + 1;
                    }
                    else
                    {
                        countDictionary.Add(tmp, 1);
                    }
                }                               
                appearList.Add((Dictionary<string, int>)countDictionary);
            }
            return appearList;
        }
        public static GainRatioMax Gain(string[][] data, List<Dictionary<string, int>> appearList, double DecisionEntropy)
        {
            GainRatioMax gainRatioMax = new GainRatioMax();
            for (int i = 0; i < data.Length - 1; i++)
            {
                log($"Atrybut {i + 1} ");
                var InfoValue = Info(data[i], appearList[i], data[data.Length - 1]);
                log($"Info: {InfoValue}");
                var Gain = DecisionEntropy - InfoValue;
                log($"Gain : {Gain}");
                var SplitInfo = Entropy(appearList[i]);
                log($"SplitInfo : {SplitInfo}");
                var GainRatio = Gain / SplitInfo;
                log($"GainRatio : {GainRatio}");
                if (gainRatioMax.GainRatio < GainRatio)
                {
                    gainRatioMax.GainRatio = GainRatio;
                    gainRatioMax.Index = i ;
                }
            }
            return gainRatioMax;
        }
        public static GainRatioMax ChooseBestAttribute(string [][] data)
        {
            int[] possibleValues = new int[data[0].Length];
            Array.Fill(possibleValues, 0);
            var appearList = GetAppearList(data);            
            var dataFlip = TableFlip(data);
            log("Podstawowa");
            
            log("Unikalne wartosci");
            foreach (var ele in possibleValues)
                log($"{ele} ", 2);
            log("");
            var decisionEntropy = Entropy(appearList[data[0].Length - 1]);
            log($"Entropia dla decyzji: {decisionEntropy}");
            var gainRatioMax = Gain(dataFlip, appearList, decisionEntropy);
            log($"GainRatioMax : {gainRatioMax.GainRatio} dla atrybutu {gainRatioMax.Index}");
            if (gainRatioMax.GainRatio == Double.NaN)
                { gainRatioMax.GainRatio = 0; }
            return gainRatioMax;
        }
        public static string[][] TableFlip(string[][] data)
        {
            string[][] dataFlip = new string[data[0].Length][];
            for (int i = 0; i < data[0].Length; i++)
            {
                string[] kolumna = new string[data.Length];
                for (int j = 0; j < data.Length; j++)
                {
                    var tmp = data[j][i];
                    kolumna[j] = tmp;

                }
                dataFlip[i] = kolumna;
                
            }
            return dataFlip;
        }

        public static Node BuildTree(string[][] data)
        {
            Node n = new Node();
            n.data = data;
            if (ChooseBestAttribute(data).GainRatio > 0)
            {
                n.gr = ChooseBestAttribute(data).GainRatio;
                n.index = ChooseBestAttribute(data).Index;
                string[][] dataflip = TableFlip(data);
                var unique = dataflip[n.index].Distinct();
                foreach (var row in unique)
                {
                    var indexes = dataflip[n.index].Select((s, i) => new { i, s })
                                    .Where(t => t.s == row)
                                    .Select(t => t.i)
                                    .ToList();

                    int i = 0;
                    string[][] new_data = new string[indexes.Count][];
                    foreach (var index in indexes)
                    {
                        new_data[i] = data[index];
                        i += 1;
                    }
                    n.value.Add(row);
                    n.node.Add(BuildTree(new_data));
                }
            }
            else
            {
                if (data.ElementAtOrDefault(0) != null)
                {
                    List<string> decisions = new List<string>();
                    for (int i = 0; i < data.Length; i++)
                    {
                        decisions.Add(data[i][data[0].Length - 1]);
                        
                    }
                    n.decision = decisions.Max();
                }
                else
                    n.decision = "Nie umiem podjąć decyzji";
            }
            return n;
        }
        public static void DrawTree(string prefix,Node tree,string value)
        {

            Console.WriteLine("{0} {1}",prefix, ((value =="") ? "" : $"{value} => ")  +((tree.decision is null)? $"Atrybut a{tree.index+1}":tree.decision));
            foreach (Node n in tree.node)
            {
                if (tree.node.IndexOf(n) == tree.node.Count - 1)
                    DrawTree(prefix + "    ", n, tree.value[tree.node.IndexOf(n)]);
                else
                    DrawTree(prefix + "   |", n, tree.value[tree.node.IndexOf(n)]);
            }

        }
        static void Main(string[] args)
        {
            string filepath = "d:/Zajecia/jKoz/";
            
            filepath += "car.data";
            List<char> spliters = new List<char>();

            spliters.Add(';');
            spliters.Add(':');
            spliters.Add('.');
            spliters.Add(' ');
            spliters.Add(',');           
            filepath = "";
            while (!File.Exists(filepath))
            {
                if (filepath == "")
                    Console.WriteLine("Wskaż plik");
                else
                    Console.WriteLine("Plik nie istnieje");
                filepath = Console.ReadLine();
            }
            Console.WriteLine("1. Wczytaj plik z separatorami ';' | ':' | '.' | ' ' | ',' | '  ' ");
            Console.WriteLine("2. Wskaż swój własny separator");
            Console.WriteLine("3. Odczytaj separator z pierwszej lini");
            
            var key = Console.ReadKey(true);
            switch(key.Key)
            {
                case ConsoleKey.D2:
                    Console.WriteLine("Podaj separator: ");
                    spliter = Console.ReadLine();
                    data = File.ReadLines(filepath).Select(line => line.Split(spliter)).ToArray();
                    break;
                case ConsoleKey.D3:
                     spliter = File.ReadLines(filepath).First();
                     data = File.ReadLines(filepath).Skip(1).Select(line => line.Split(spliter)).ToArray();
                    break;
                default :
                    string lines = File.ReadLines(filepath).ToString();
                    spliter = FindSpliter(spliters, lines);
                    data = File.ReadLines(filepath).Select(line => line.Split(spliter)).ToArray();
                    break;

            }
            
            
            Tree = BuildTree(data);
            DrawTree("", Tree,"");
            Console.ReadKey();
        }

        private static string FindSpliter(List<char> spliter,string lines)
        {
            int countSpliter = 0;
            char bestSpliter;
            bestSpliter = ',';
            foreach (var s in spliter)
            {
                int tmp;
                tmp = lines.Count(l => l == s);
                if (tmp > countSpliter)
                    countSpliter = tmp;
                    bestSpliter = s;
            }
            return bestSpliter.ToString();
        }
    }
}
