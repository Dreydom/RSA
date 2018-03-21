using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
namespace RSA
{
    class Program
    {
        static Encoding cp866 = Encoding.GetEncoding("cp866");
        private static BigInteger GCD(BigInteger a, BigInteger b)
        {
            if (a == 0)
                return b;
            if (b == 0)
                return a;
            if (a > b)
                return GCD(a % b, b);
            else
                return GCD(a, b % a);
        }
        private static BigInteger Coprime(BigInteger a)
        {
            BigInteger i = 2;
            while (i < a)
            {
                if (GCD(a, i) == 1) return i;
                else i++;
            }
            return -1;
        }
        private static BigInteger Inverse(BigInteger A, BigInteger M)
        {
            BigInteger B = M;
            if (A < B) //if A less than B, switch them
            {
                BigInteger temp = A;
                A = B;
                B = temp;
            }
            BigInteger r = B;
            BigInteger q = 0;
            BigInteger x0 = 1;
            BigInteger y0 = 0;
            BigInteger x1 = 0;
            BigInteger y1 = 1;
            BigInteger x = 0, y = 0;
            while (r > 1)
            {
                r = A % B;
                q = A / B;
                x = x0 - q * x1;
                y = y0 - q * y1;
                x0 = x1;
                y0 = y1;
                x1 = x;
                y1 = y;
                A = B;
                B = r;
            }
            return (y+M)%M;
        }
        private static List<string> Divider(string str, int blockLength)
        {
            List<string> Blocks = new List<string>(str.Length / blockLength + 1);
            for (int i = 0; i < str.Length; i += blockLength)
            {
                if (str.Length - i > blockLength)
                    Blocks.Add(str.Substring(i, blockLength));
                else
                    Blocks.Add(str.Substring(i, str.Length - i) + new String('\0', blockLength - (str.Length - i)));
            }
            return Blocks;
        }
        private static List<BigInteger> ToNum(List<string>blocks)
        {
            List<BigInteger> codeblocks = new List<BigInteger>();
            foreach (string block in blocks)
            {
                int i = block.Length - 1; //3
                BigInteger codeblock = 0;
                foreach (char letter in block)
                {
                    codeblock += (BigInteger)(cp866.GetBytes(new char[] { letter })[0] * Math.Pow(256, i));
                    i--;
                }
                codeblocks.Add(codeblock);
            }
            return codeblocks;
        }
        private static List<string> ToText(List<BigInteger>codeblocks)
        {
            List<string> textblocks = new List<string>();

            foreach (BigInteger codeblock in codeblocks)
            {
                string textblock = "";
                BigInteger number = codeblock;
                do
                {
                    textblock = textblock + cp866.GetChars(new byte[] { (byte)(number % 256) })[0];
                    number = number / 256;
                }
                while (number != 0);
                textblock = new string(textblock.Reverse().ToArray());
                textblocks.Add(textblock);
            }
            return textblocks;
        }
        private static List<BigInteger> Encrypt(List<BigInteger>codeblocks, BigInteger e, BigInteger n)
        {
            List<BigInteger> encryptedblocks = new List<BigInteger>();
            foreach (BigInteger codeblock in codeblocks)
            {
                encryptedblocks.Add(BigInteger.ModPow(codeblock, e, n));
            }
            return encryptedblocks;
        }
        private static List<BigInteger> Decrypt(List<BigInteger> encryptedcodeblocks, BigInteger d, BigInteger n)
        {
            List<BigInteger> decryptedblocks = new List<BigInteger>();
            foreach (BigInteger encryptedcodeblock in encryptedcodeblocks)
            {
                decryptedblocks.Add(BigInteger.ModPow(encryptedcodeblock, d, n));
            }
            return decryptedblocks;
        }
        static void Main(string[] args)
        {
            BigInteger p = BigInteger.Parse("4294967311");
            BigInteger q = BigInteger.Parse("4294967357");
            BigInteger n = p * q;
            BigInteger m = (p - 1) * (q - 1);
            BigInteger d = Coprime(m);
            BigInteger e = Inverse(d, m);

            Console.WriteLine("p = {0}", p);
            Console.WriteLine("q = {0}", q);
            Console.WriteLine("n = {0}", n);
            Console.WriteLine("m = {0}", m);
            Console.WriteLine("d = {0}", d);
            Console.WriteLine("e = {0}", e);
            Console.Write("Введите исходный текст: ");
            string input = Console.ReadLine();
            List<string> blocks = Divider(input, 4);
            int length = blocks.Count; //количество блоков
            List<BigInteger> codeblocks = ToNum(blocks);
            List<BigInteger> encryptedblocks = Encrypt(codeblocks, e, n);
            List<BigInteger> decryptedblocks = Decrypt(encryptedblocks, d, n);
            List<string> textblocks = ToText(decryptedblocks);
            Console.WriteLine("{0,-5}│{1,14}│{2,20}│{3,14}│{4,-5}", "Блок", "Число", "Зашированное", "Расшифрованное", "Блок");
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine(@"─────┼──────────────┼────────────────────┼──────────────┼──────");
            for (int i = 0; i<length; i++)
            {
                Console.WriteLine("{0,-5}│{1,14}│{2,20}│{3,14}│{4,-5}", blocks[i], codeblocks[i], encryptedblocks[i], decryptedblocks[i], textblocks[i]);
            }
            Console.ReadLine();
        }
    }
}
