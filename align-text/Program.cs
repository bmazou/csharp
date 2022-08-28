using Microsoft.Win32.SafeHandles;
using System.Data;
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

        static string FillGaps(List<string> words, int gapCount) {
            string alignedLine = "";
            int seperationCount = words.Count - 1;

            for (int i = 0; i < seperationCount; i++) {
                words[i] += " "; 
            }
            
            for (int i = 0; i < gapCount; i++) {
                words[i % seperationCount] += " "; 
            }

            foreach (string word in words) {
                alignedLine += word;
            }

            return alignedLine;
        }

        static (string, string) LineOverflow(List<string> words) {
            string lastWord = words.Last();
            int lastWordLength = lastWord.Length;

            words.RemoveAt(words.Count - 1);

            string alignedLine = FillGaps(words, lastWordLength + 1);
            return (alignedLine, lastWord);
        }

        static (string, string) HandleBatch(StreamReader sr, string prevLastWord, int rowLen, int rep) {
            string line = prevLastWord;
            List<string> words;
            for (int i = 0; i < rowLen; i++) {
                int charInt = sr.Read();
                if (charInt == -1) {
                    return ("","");              //TODO Tady ať to vrátí něco jako null ať vim že je konec
                }
                char ch = (char)charInt;
                line += ch;
            }
            char nextChar = (char)sr.Peek();
            if (separators.Contains(nextChar)) {   
                // Next char is a separator, so move the reader  
                sr.Read();      
                // and return the line without any change 
                return (line, "");                
            }else {
                words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();     
                // var overflowOutput = LineOverflow(words);
                (string alignedLine, string lastWord) = LineOverflow(words);

                // if (rep == 5) {
                //     Console.WriteLine(lastWord);
                // }

                return (alignedLine, lastWord);
            }
        }

        //TODO Samotný odstavce upravuje skvěle, ten jen vyřešit tu indentaci apod. 
        static void AlignFile(string inFileName, string outFileName, int rowLen)
        {
            using (StreamReader sr = new StreamReader(inFileName)) {
                using (StreamWriter sw = new StreamWriter(outFileName)) {
                    int i = 0;
                    string prevLastWord = "";
                    while (i < 10) {
                        (string alignedLine, string lastWord) = HandleBatch(sr, prevLastWord, rowLen - prevLastWord.Length, i);
                        
                        // if (lastWord == "") {
                        //     Console.WriteLine(alignedLine);
                        //     Console.WriteLine(i);
                        // }

                        if (i == 5) {
                            // Console.WriteLine(alignedLine);
                            // Console.WriteLine(lastWord);
                        }

                        sw.Write(alignedLine);
                        sw.WriteLine();

                        i++;
                        prevLastWord = lastWord;
                    }

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
