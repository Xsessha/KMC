using System;

class Program
{
    static string ConvertInteger(int number, int baseValue)
    {
        if (baseValue < 2 || baseValue > 16)
            throw new ArgumentException("Основа системи повинна бути від 2 до 16.");

        if (number == 0)
            return "0";

        bool negative = number < 0;
        long n = Math.Abs((long)number);

        string digits = "0123456789ABCDEF";
        string result = "";

        while (n > 0)
        {
            int remainder = (int)(n % baseValue);
            result = digits[remainder] + result;
            n /= baseValue;
        }

        return negative ? "-" + result : result;
    }

    static string ConvertFraction(double fraction, int baseValue, int count)
    {
        string result = "";

        for (int i = 0; i < count; i++)
        {
            fraction *= baseValue;

            int digit = (int)fraction;
            result += digit;

            fraction -= digit;
        }

        return result;
    }

    static string To8Bit(int number)
    {
        byte value = unchecked((byte)number);

        return Convert.ToString(value, 2).PadLeft(8, '0');
    }

    static bool CheckOverflow(int a, int b)
    {
        int sum = a + b;

        return sum < -128 || sum > 127;
    }

    static string FloatToBits(float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        string result = "";

        foreach (byte b in bytes)
        {
            result += Convert.ToString(b, 2).PadLeft(8, '0');
        }

        return result;
    }

    static string FloatToHex(float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        string result = "";

        foreach (byte b in bytes)
        {
            result += b.ToString("X2");
        }

        return result;
    }

    static void Main()
    {

        int N = 87;
        double F = 0.95;

        int A = -100;
        int B = -1;

        float C = 256.5f;

        Console.WriteLine("       ПРАКТИЧНА РОБОТА №1");
        Console.WriteLine("       Варіант 12");


        Console.WriteLine("\n1. Переведення цілого числа N = 87");

        string binary = ConvertInteger(N, 2);
        string octal = ConvertInteger(N, 8);
        string hexadecimal = ConvertInteger(N, 16);

        Console.WriteLine($"Двійкова система:       {binary}");
        Console.WriteLine($"Вісімкова система:      {octal}");
        Console.WriteLine($"Шістнадцяткова система: {hexadecimal}");


        Console.WriteLine("\n2. Переведення дробу F = 0.95");

        string fractionBinary = ConvertFraction(F, 2, 6);

        Console.WriteLine($"Двійкова система (6 розрядів): 0.{fractionBinary}");


        Console.WriteLine("\n3. Додавання 8-бітних чисел");

        string aBits = To8Bit(A);
        string bBits = To8Bit(B);

        int sum = A + B;

        string sumBits = To8Bit(sum);

        bool overflow = CheckOverflow(A, B);

        Console.WriteLine($"A = {A}:  {aBits}");
        Console.WriteLine($"B = {B}:  {bBits}");
        Console.WriteLine($"Sum = {sum}: {sumBits}");
        Console.WriteLine($"Прапорець переповнення (Overflow): {overflow}");


        Console.WriteLine("\n4. Число C = 256.5 у форматі IEEE 754");

        string ieeeBits = FloatToBits(C);
        string ieeeHex = FloatToHex(C);

        Console.WriteLine($"32 біти: {ieeeBits}");
        Console.WriteLine($"Hex: 0x{ieeeHex}");

        Console.WriteLine("Знак S = 0");
        Console.WriteLine("Порядок E = 10000111 (p = 8)");
        Console.WriteLine("Мантиса M = 1.000000001...");

        float restoredValue = BitConverter.Int32BitsToSingle(
            BitConverter.SingleToInt32Bits(C)
        );

        Console.WriteLine($"Відновлене значення: {restoredValue}");


        Console.WriteLine("\n5. Похибка операцій з плаваючою комою");

        double x = 0.1;
        double y = 0.2;
        double expected = 0.3;

        double result = x + y;

        Console.WriteLine($"0.1 + 0.2 = {result}");
        Console.WriteLine($"0.1 + 0.2 = 0.3: {result == expected}");

        double difference = Math.Abs(result - expected);

        Console.WriteLine($"Абсолютна похибка: {difference:E}");

        Console.WriteLine("Програму завершено.");
    }
}