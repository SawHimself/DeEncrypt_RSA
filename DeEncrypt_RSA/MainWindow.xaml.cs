using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Math;

namespace DeEncrypt_RSA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        long[] OpenKey = new long[2];
        long[] SecretKey = new long[2];
        long[] GetOpenKey = new long[2];
        public MainWindow()
        {
            InitializeComponent();
        }
        private void GenerateKey(object sender, EventArgs e)
        {

            Random random = new Random();

            long p = random.Next(1000, 151850);
            p += random.Next(1000, 1518500);

            long q = random.Next(1000, 151850);
            q += random.Next(1000, 151850);

            p = FindTheNearestPrime(p);
            q = FindTheNearestPrime(q);
            long N = p * q;

            long EulerFunction = (p - 1) * (q - 1);

            //Выбираем число e 
            // e взаимнопросто с функцией Эйлера
            long E = random.Next(10000000, 90000000);
            while(true)
            {
                E = FindTheNearestPrime(E);
                if(EulerFunction % E == 0)
                {
                    E = random.Next(1000000, 9000000);
                }
                else
                {
                    break;
                }
            }

            long d = ExtendedEuclideanAlgorithm(EulerFunction, E);

            OpenKey[0] = E;
            OpenKey[1] = N;
            SecretKey[0] = d;
            SecretKey[1] = N;

            long test1 = d * E % EulerFunction;
            OpenKeyLabel.Text = OpenKey[0].ToString() + "-" + OpenKey[1].ToString();
            SecreKeyLabel.Content = SecretKey[0].ToString() + "-" + SecretKey[1].ToString();
        }
        private long FindTheNearestPrime(long x)
        {
            bool flag = false;
            for (long i = x; i < long.MaxValue; i++)
            {
                flag = false;
                for (long j = 2; j < Sqrt(i) + 1; j++)
                {
                    if (i % j == 0)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    x = i;
                    return x;
                }
            }
            return 0;
        }
        private long ExtendedEuclideanAlgorithm(long a, long b)
        {
            long d = 0;
            long k = 0;
            while(true)
            {
                k++;
                if ((k * a + 1) % b == 0)
                {
                    d = (k * a + 1) / b;
                    break;
                }
            }
            return d;
        }

        private List<long> Encrypt(string openText, long E, long N)
        {
            List<long> Ciphertext = new List<long>();
            foreach (char c in openText)
            {
                long x = (int)c;
                BigInteger res1 = BigInteger.ModPow(x, E, N);
                x = (long)res1;
                Ciphertext.Add(x);
            }
            return Ciphertext;
        }

        private void EncryptText(object sender, EventArgs e)
        {
            int x = 0;
            foreach (char c in ReceivedKey.Text)
            {
                if (c == '-')
                {
                    x++;
                }
                else
                {
                    GetOpenKey[x] *= 10;
                    GetOpenKey[x] += c - '0';
                }
            }
            string forEncrypt = EnteredText.Text;
            string result = "";
            List<long> Ciphertext = Encrypt(forEncrypt, GetOpenKey[0], GetOpenKey[1]);
            for(int i = 0; i < Ciphertext.Count; i++)
            {
                result += Ciphertext[i].ToString();
                result += " ";
            }
            ResultText.Text = result;
        }

        private string Decrypt(List<long> Ciphertext, long d, long N)
        {
            StringBuilder sb = new StringBuilder();
            foreach (long c in Ciphertext)
            {
                long x = c;
                BigInteger result = BigInteger.ModPow(x, d, N);

                try 
                { 
                    char a = (char)result;
                    sb.Append(a);
                }
                catch { };
            }
            return sb.ToString();
        }
        private void DecryptText(object sender, EventArgs e)
        {
            string forDecrypt = EnteredText.Text;
            long buf = 0;
            List<long> message = new List<long>();
            foreach(char x in forDecrypt)
            {
                if(x == ' ')
                {
                    if (buf != 0)
                    {
                        message.Add(buf);
                    }
                    buf = 0;
                }
                else
                {
                    buf *= 10;
                    buf += x - '0';
                }
            }
            ResultText.Text = Decrypt(message, SecretKey[0], SecretKey[1]);
        }
    }
}
