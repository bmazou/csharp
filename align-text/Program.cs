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
                return (num > 0);
            }
            catch {
                return false;
            }
        }

        static bool IsSepartor(char ch) {
            return separators.Contains(ch);
        }

        //* Reads all following whitespaces, until it hits a character 
        static void ReadAllSeparators(StreamReader sr) {
            while (true) {
                char nextChar = (char)sr.Peek();
                if (IsSepartor(nextChar)) {   
                    // Next char is a separator, so move the reader  
                    sr.Read();
                }else {
                    return;
                }
            }
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

        static (string, string) HandleBatch(StreamReader sr, string prevLastWord, int rowLen) {
            string line = prevLastWord;
            List<string> words;
            for (int i = 0; i < rowLen; i++) {
                int charInt = sr.Read();
                if (charInt == -1) {
                    var ret = line.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();     
                    line = String.Join(" ", ret);
                    return (line, null);              
                }
                char ch = (char)charInt;
                if (IsSepartor(ch)) {
                    char nextCh = (char)sr.Peek();
                    if (IsSepartor(nextCh)) {
                        ReadAllSeparators(sr);
                        line += "\n";
                        return (line, "");
                    }
                     
                    ch = ' ';    // In case ch is '\n' or '\t', we want it to be ' '
                }
                line += ch;
            }

            char nextChar = (char)sr.Peek();
            if (IsSepartor(nextChar)) {   
                // Next char is a separator, so move the reader  
                sr.Read();      
                // and return the line without any change 
                return (line, "");                
            }else {
                words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();     

                if (IsSepartor(line[line.Length - 1])) {
                    string alignedLine = FillGaps(words, 1);
                    return (alignedLine, "");
                }


                // (string alignedLine, string lastWord) = LineOverflow(words);
                return LineOverflow(words);

            }
        }

        static void AlignFile(string inFileName, string outFileName, int rowLen)
        {
            using (StreamReader sr = new StreamReader(inFileName)) {
                using (StreamWriter sw = new StreamWriter(outFileName)) {
                    string prevLastWord = "";
                    while (true) {
                        (string alignedLine, string lastWord) = HandleBatch(sr, prevLastWord, rowLen - prevLastWord.Length);

                        // Console.WriteLine(lastWord);
                        sw.Write(alignedLine);
                        sw.WriteLine();

                        if (lastWord is null) {
                            break;
                        }
                        if (lastWord == "\n" || lastWord == "\t" || lastWord == " ")
                            lastWord = "";
                        prevLastWord = lastWord;
                    }

                }
            }
        }

        static void Main(string[] args)
        {
            if (!ValidArgs(args)) {
                Console.WriteLine("Argument Error");
                return;
            }

            // Podmínka pro neexistující soubor
            try {
                AlignFile(args[0], args[1], Convert.ToInt32(args[2]));
            }

            catch {
                Console.WriteLine("File Error");
            }
            // AlignFile("input.txt", "out.txt", 40);

        }

    }
}
