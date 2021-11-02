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
            
            Console.WriteLine(GetResult(firstNum, secondNum));
        }

        private static string GetResult(string first, string second)
        {
            if (first == second) // если числа равны - сразу возвращаем нули
            {
                return $"Binary: 0000000\nDecimal: 0\nBinary evenness: 0"; //+
                //$"\nTransfer: 0\nOverflow: {overflow}";
            }
            // списки для цифр бинарного числа
            var firstBinary = new List<float>();
            var secondBinary = new List<float>();
            
            var mayOverflow = false; // флаг, определяющий переполнение
            var firstOver = false; // изначальаня переполненность первого числа
            var secondOver = false; // изначальная переполненность второго числа
            var result = new List<float>(); // список цифр довичного результата
            var temp = 0; // число "в уме" для суммации
            var transfer = 0; // перенос 
            var sign = int.Parse(first) > int.Parse(second) ? "" : "-"; // знак результата
            
            if (Math.Abs(int.Parse(second)) > Math.Abs(int.Parse(first))) // меняем первое и второе число местами, если по модулю второе больше первого (необходимо для адекватных вычислений)
            {
                (first, second) = (second, first); // краткая запись замены значений переменных
            }
            // блок-развилка под все варианты знаков чисел (описание вызываемых методов в самих методах)
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
            
            // цикл складывания по правилу "столбика" двух бинарных чисел
            for (var i = 0; i < firstBinary.Count; i++)
            {
                var mid = firstBinary[i] + secondBinary[i] + temp; 
                // если при складывании двух цифр разных чисел и числа "в уме" получится 2, то в результат записываем 0 и 1 "в уме"
                // если получится 3, то в результат 1 и снова 1 "в уме"
                // по дефолту "в уме" становится равным 0, в результат добавляется сумма цифр и числа "в уме"
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
            // если после цикла сложения цифр, число "в уме" осталось и результат НЕ может быть переполненным
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
            // если после цикла сложения цифр, число "в уме" осталось и оно может быть переполненным
            else if (temp == 1 && mayOverflow)
            {
                result.Add(1);
                transfer = 1;
            }
            var BSing = sign == "" ? 0 : 1; // определяем знак числа в бинарном варианте 
            
            result.Reverse();// переворачиваем список результата, так как складывали цифры в обратном порядке (принцип "столбика")
            
            var BinaryRes = string.Join("", result); // собираем из элементов списка строку с пустым разделителем
            var DecRes = GetDecFromBinary(BinaryRes); // переводим в десятичный вариант
            var overflow = firstOver || secondOver ? 1 : 0; // определяем для ответа, было ли переполнение
            var evenness = CheckForEven(BSing+BinaryRes) ? 1 : 0; // определяем десятичную четность
                
            return $"Binary: {BSing}{BinaryRes}\nDecimal: {sign}{DecRes}\nBinary evenness: {evenness}" +
                   $"\nTransfer: {transfer}\nOverflow: {overflow}";
        }

        private static List<float> GetBinaryFromDec(string dec)
        {
            // метод переводит десятичное число в двоичный вариант по стандартным правилам перевода
            var num = float.Parse(dec);
            var result = new List<float>();

            while (num != 0)
            {
                result.Add(num % 2);
                num = (int)Math.Floor(num / 2);
            }

            while (result.Count < 6)
            {
                result.Add(0);
            }
            return result;
        }
        private static double GetDecFromBinary(string bin)
        {
            // метод переводит двоичное число в десятичное по стандартным правилам
            var temp = bin.Reverse(); // переворачиваем полученную строку, чтобы корректно по правилам ее перевести
            return temp.Select((t, i) => int.Parse(t.ToString()) * Math.Pow(2, i)).Sum(); // каждое число получившегося перевернутого списка умножаем на 2 в степени порядкового номера числа и в итоге возвращаем сумму результатов
        }
        private static void CheckForOverflow(ref List<float> x, ref List<float> y,ref bool xb,ref bool yb)
        {
            // метод проверяет на переполнение и добавляет нули там, где они нужны (ниже подробнее)
            // на вход принимает ССЫЛКАМИ (ref) списки цифр двоичных чисел и булевые переменные-флаги переполнения
            
            if (x.Count > 6) xb = true; // если цифр больше семи - переполнились
            if (y.Count > 6) yb = true; // аналогично

            // условия дли добавления дополнительных нулей, если одно из числе по разрядности больше другого
            // это нужно для корректной суммации и чтобы не получить ошибкой в консоль
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
            // проверка на бинарную четность числа (по стандартному правилу)
            var zeros = 0;
            var ones = 0;
            
            foreach (var t in result)
            {
                if (t == '0') zeros++;
                else ones++;
            }

            return zeros % 2 == 0 || ones % 2 == 0; // если нулей четное количество или единиц четное количество - возвращаем истину
        }
    }
}
