using System;
using System.Linq;
using System.Collections.Generic;

namespace BinaryCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            var firstNum = Console.ReadLine();
            var secondNum = Console.ReadLine();
            //Console.WriteLine((int)Math.Floor(1d / 2d));
            //Console.WriteLine(GetDecFromBinary(firstNum));
            Console.WriteLine(GetResult(firstNum, secondNum));
            //Console.WriteLine(GetBinaryFromDec(firstNum));
        }

        private static string GetResult(string first, string second)
        {
            if (first == second)
            {
                return "Binary: 0000000\nDecimal: 0";
            }
            var firstBinary = new List<float>();
            var secondBinary = new List<float>();
            var mayOverflow = false;
            var firstOver = false;
            var secondOver = false;
            var result = new List<float>();
            var temp = 0;
            var transfer = 0;
            var sign = int.Parse(first) > int.Parse(second) ? "" : "-";
            
            if (Math.Abs(int.Parse(second)) > Math.Abs(int.Parse(first)))
            {
                (first, second) = (second, first);
            }
            
            if (first.Contains('-') && second.Contains('-'))
            {
                firstBinary = GetBinaryFromDec(first.Split('-')[1]).ToList();
                secondBinary = GetBinaryFromDec(second.Split('-')[1]);
                CheckForOverflow(ref firstBinary, ref secondBinary, ref firstOver, ref secondOver);
                secondBinary = secondBinary.Select(x => x == 1 ? 0f : 1f).ToList();
            }
            else if (first.Contains('-'))
            {
                firstBinary = GetBinaryFromDec(first.Split('-')[1]).ToList();
                secondBinary = GetBinaryFromDec(second).ToList();
                mayOverflow = true;
                CheckForOverflow(ref firstBinary, ref secondBinary, ref firstOver, ref secondOver);
            }
            else if (second.Contains('-'))
            {
                firstBinary = GetBinaryFromDec(first);
                secondBinary = GetBinaryFromDec(second.Split('-')[1]).ToList();
                mayOverflow = true;
                CheckForOverflow(ref firstBinary, ref secondBinary, ref firstOver, ref secondOver);
            }
            else
            {
                firstBinary = GetBinaryFromDec(first);
                secondBinary = GetBinaryFromDec(second);
                CheckForOverflow(ref firstBinary, ref secondBinary, ref firstOver, ref secondOver);
                secondBinary = secondBinary.Select(x => x == 1 ? 0f : 1f).ToList();
            }
            
            for (var i = 0; i < firstBinary.Count; i++)
            {
                var mid = firstBinary[i] + secondBinary[i] + temp;
                switch (mid)
                {
                    case 2:
                        temp = 1;
                        result.Add(0);
                        break;
                    case 3:
                        temp = 1;
                        result.Add(1);
                        break;
                    default:
                        result.Add(firstBinary[i] + secondBinary[i] + temp);
                        temp = 0;
                        break;
                }
            }

            if (temp == 1 && !mayOverflow)
            {
                for (var i = 0; i < result.Count; i++)
                {
                    if (result[i] == 0)
                    {
                        result[i] = 1;
                        break;
                    }
                    result[i] = 0;
                }
            }
            else if (temp == 1 && mayOverflow)
            {
                result.Add(1);
                transfer = 1;
            }

            result.Reverse();
            var BinaryRes = string.Join("", result);
            var DecRes = GetDecFromBinary(BinaryRes);
            var BSing = sign == "" ? 0 : 1;
            var overflow = firstOver || secondOver ? 1 : 0;
            var evenness = CheckForEven(BinaryRes) ? 1 : 0;
                
            return $"Binary: {BinaryRes}\nBinary sign: {BSing}\nDecimal: {sign}{DecRes}\nBinary evenness: {evenness}" +
                   $"\nTransfer: {transfer}\nOverflow: {overflow}";
        }

        private static List<float> GetBinaryFromDec(string dec)
        {
            var num = float.Parse(dec);
            var result = new List<float>();

            while (num != 0)
            {
                result.Add(num % 2);
                num = (int)Math.Floor(num / 2);
            }

            while (result.Count < 7)
            {
                result.Add(0);
            }
            //result.Reverse();
            return result;
        }
        private static double GetDecFromBinary(string bin)
        {
            var temp = bin.Reverse();
            return temp.Select((t, i) => int.Parse(t.ToString()) * Math.Pow(2, i)).Sum();
        }
        private static void CheckForOverflow(ref List<float> x, ref List<float> y,ref bool xb,ref bool yb)
        {
            if (x.Count > 7) xb = true;
            if (y.Count > 7) yb = true;

            if (x.Count > y.Count)
            {
                var diff = x.Count - y.Count;
                for (var i = 0; i < diff; i++)
                {
                    y.Add(0);
                }
            }
            else if(x.Count < y.Count)
            {
                var diff = y.Count - x.Count;
                for (var i = 0; i < diff; i++)
                {
                    x.Add(0);
                }
            }
        }
        private static bool CheckForEven(string result)
        {
            var zeros = 0;
            var ones = 0;
            
            foreach (var t in result)
            {
                if (t == '0') zeros++;
                else ones++;
            }

            return zeros % 2 == 0 || ones % 2 == 0;
        }
    }
}