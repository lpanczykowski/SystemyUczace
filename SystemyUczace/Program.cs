using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SystemyUczace
{
    class Program
    {
      
       public static Boolean debug = false;

        public static void log(string text, int method = 1,int force = 0)
        {
            if ((debug && method == 1) || (force == 1))
                Console.WriteLine(text);
            else if ((debug && method == 2) || (force == 1))
                Console.Write(text);
        }
        public static void PokazTablice(string[][] array)
        {
            foreach(string[] row in array)
            {
                foreach(string value in row)
                {
                    log(value,2);
                    log(" ",2);
                }
                log("");
            }
        }
        public static double  Entropy(IDictionary dec)
        {
            double EntropyValue = 0;
            double count =0 ;
            List<int> items = new List<int>();
            foreach (DictionaryEntry de in dec)
            {
                items.Add(Convert.ToInt32(de.Value));
                count +=Convert.ToInt32(de.Value);
            }
            foreach (int i in items)
                {
                    var p = i / count;
                EntropyValue += p*Math.Log2(p) ;
                }
            return -1* EntropyValue;
        }

        // Info(a1,T) = Tnew/T * Info(New)
        public static double Info(string[] data, IDictionary dic,string[] des)
        {
            double sum = 0;
           
            foreach (DictionaryEntry de in dic)
            {
                double x = 0;
                IDictionary<string, int> count = new Dictionary<string, int>();
                x = ( Convert.ToSingle(de.Value)/ data.Length);
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
               sum+= (x * Entropy((IDictionary)count));
            }
            return sum;
        }
        static void Main(string[] args)
        {
            string filepath = "d:/Zajecia/jKoz/";
            filepath += "teststring.txt";
            string spliter = File.ReadLines(filepath).First();
            string[][] data = File.ReadLines(filepath).Skip(1).Select(line => line.Split(spliter)).ToArray();

            int[] possibleValues = new int[data[0].Length];
            Array.Fill(possibleValues, 0);

            string[][] dataFlip = new string[data[0].Length][];
            List<Dictionary<string, int>> ListofAppear = new List<Dictionary<string, int>>();
            for (int i = 0; i < data[0].Length; i++)
            {
                string[] kolumna = new string[data.Length];
                IDictionary<string, int> countDictionary  = new Dictionary<string, int>();
                for (int j = 0; j < data.Length; j++)
                {
                    var tmp = data[j][i];
                    kolumna[j] = tmp;
                    if (countDictionary.ContainsKey(tmp))
                        {
                            countDictionary[tmp] = countDictionary[tmp]+ 1;
                        }
                    else
                        {
                            countDictionary.Add(tmp, 1);
                        }
                }
                dataFlip[i] = kolumna;
                possibleValues[i] = kolumna.Distinct().Count();
                ListofAppear.Add((Dictionary<string, int>)countDictionary);
            }
          
            log("Podstawowa");
            PokazTablice(data);
            log("Odwrocona");
            PokazTablice(dataFlip);
            log("Unikalne wartosci");
            foreach (var ele in possibleValues)
                log($"{ele} ",2);
            log("");
            var DecisionEntropy = Entropy(ListofAppear[data[0].Length - 1]);
            log($"Entropia dla decyzji: {DecisionEntropy}");
            GainRatioMax gainRatioMax = new GainRatioMax(0,0);
            for (int i = 0; i < dataFlip.Length-1; i++)
            {
                log($"Atrybut {i + 1} ");
                var InfoValue = Info(dataFlip[i], ListofAppear[i], dataFlip[dataFlip.Length - 1]);
                log($"Info: {InfoValue}");
                var Gain = DecisionEntropy - InfoValue;
                log($"Gain : {Gain}");
                var SplitInfo = Entropy(ListofAppear[i]);
                log($"SplitInfo : {SplitInfo}");
                var GainRatio = Gain / SplitInfo;
                log( $"GainRatio : {GainRatio}");
                if (gainRatioMax.GainRatio < GainRatio)
                {
                    gainRatioMax.GainRatio = GainRatio;
                    gainRatioMax.Index = i+1;
                }
            }

            log($"GainRatioMax : {gainRatioMax.GainRatio} dla atrybutu {gainRatioMax.Index}",1,1);

            Console.ReadKey();
        }
    }
}
