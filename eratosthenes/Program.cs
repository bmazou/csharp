using System.Collections.Generic;
using System.IO;
using System;

namespace eratosthenes
{
    static class G {
        public static ulong n = 330829;
        public static bool[] nums = new bool[n+1]; 
    }    

    class Program
    {

        static void flag_num(ulong num) {
            ulong index = (ulong)Math.Pow(num, 2);

            while (index <= G.n) {
                G.nums[index] = false;
                index += num;
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine(G.n);
            for (ulong i = 0; i < G.n; i++) {
                G.nums[i] = true;
            } 

            ulong suma = 0;
            for (ulong i = 2; i < G.n; i++) {
                if (G.nums[i]) {
                    suma += 1;
                    if (Math.Pow(i,2) < G.n) {
                        flag_num(i);
                    } 
                    
                    Console.WriteLine(i);
                }
            }

            Console.WriteLine(suma);

        }
    }
}   
