using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace align
{
    class Program
    {
        static char[] separators = new char[] {'\t', ' ', '\n'};       // Bílé znaky


        static bool ValidArgs(string[] args) 
        {
            if (args.Length != 3) 
                return false;
            
            try {
                int num = Int32.Parse(args[2]);
                if (num <= 0) 
                    return false;
            }
            catch {
                return false;
            }

            return true;
        }

        static void FillGaps(List<string> words, int gapCount) {
            int seperationCount = words.Count - 1;

            for (int i = 0; i < seperationCount; i++) {
                words[i] += " "; 
            }
            
            for (int i = 0; i < gapCount; i++) {
                words[i % seperationCount] += " "; 
            }

            foreach (string word in words) {
                Console.Write(word);
            }

        }

        static string LineOverflow(List<string> words) {
            string lastWord = words.Last();
            int lastWordLength = lastWord.Length;

            words.RemoveAt(words.Count - 1);

            FillGaps(words, lastWordLength + 1);
            // foreach (string word in words) {
            //     Console.WriteLine(word);
            // }    
            return lastWord;
        }

        static string HandleBatch(StreamReader sr, StreamWriter sw, int rowLen) {
            string line = "";
            List<string> words;
            for (int i = 0; i < rowLen; i++) {
                int charInt = sr.Read();
                if (charInt == -1) {
                    return "";              //TODO Tady ať to vrátí něco jako null ať vim že je konec
                }
                char ch = (char)charInt;
                line += ch;
            }

            char nextChar = (char)sr.Peek();
            if (separators.Contains(nextChar)) {
                return "";
            }else {
                words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();     
                LineOverflow(words);
                return "";


            }

        }

        static void AlignFile(string inFileName, string outFileName, int rowLen)
        {
            using (StreamReader sr = new StreamReader(inFileName)) {
                using (StreamWriter sw = new StreamWriter(outFileName)) {
                    HandleBatch(sr, sw, rowLen);
                    // HandleBatch(sr, sw, rowLen);
                }
            }
        }

        static void Main(string[] args)
        {
            // if (!ValidArgs(args)) {
            //     Console.WriteLine("Argument Error");
            //     return;
            // }

            // // Podmínka pro neexistující soubor
            // try {
                // AlignFile(args[0], args[1], Convert.ToInt32(args[2]));
            // }

            // catch {
            //     Console.WriteLine("File Error");
            // }
            AlignFile("input.txt", "out.txt", 40);

        }

    }
}
