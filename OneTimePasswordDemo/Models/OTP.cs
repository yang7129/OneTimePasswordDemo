using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace OneTimePasswordDemo.Models
{
    public class OTP
    {
        private static readonly DateTime m_UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const int Effective_Time = 30; //時效過期
        private string m_authenticationCode; //Code
        public DateTime m_expiry; //時間截止
        private int _digits = 6; //共產生多少位數
        private OneTimePasswordAlgorithm Algorithm = OneTimePasswordAlgorithm.SHA512;

        public enum OneTimePasswordAlgorithm
        {
            Sha1,
            SHA256,
            SHA384,
            SHA512
        }
        public OTP(string base32Secret)
        {
            OneTimePassword(base32Secret, null);
        }
        public OTP(string base32Secret, DateTime AddUtcTime)
        {
            OneTimePassword(base32Secret, AddUtcTime);
        }
        public void OneTimePassword(string Secret, DateTime? AddUtcTime)
        {
            byte[] secretBytes = Base32.ToByteArray(Secret);

            DateTime utcNow = AddUtcTime ?? DateTime.UtcNow;//當如果沒有值的時候就用現在的UTC時間
            long unixNow = ToUnixTime(utcNow);//utc轉 時間戳記
            long timestamp = Convert.ToInt64(unixNow / Effective_Time);
            byte[] timestampBytes = BitConverter.GetBytes(timestamp).ToArray();

            Array.Reverse(timestampBytes);//反向
            HMAC hmac = null;
            switch (Algorithm)
            {
                case OneTimePasswordAlgorithm.Sha1: hmac = new HMACSHA1(secretBytes); break;
                case OneTimePasswordAlgorithm.SHA256: hmac = new HMACSHA256(secretBytes); break;
                case OneTimePasswordAlgorithm.SHA384: hmac = new HMACSHA384(secretBytes); break;
                case OneTimePasswordAlgorithm.SHA512: hmac = new HMACSHA512(secretBytes); break;
            }

            byte[] hmacBytes = hmac.ComputeHash(timestampBytes);
            int offset = hmacBytes[hmacBytes.Length - 1] & 0x0F;

            var truncatedHash = new byte[] { (byte)(hmacBytes[offset + 0] & 0x7F), hmacBytes[offset + 1], hmacBytes[offset + 2], hmacBytes[offset + 3] };
            var number = BitConverter.ToInt32(truncatedHash, 0);
            int code = number % DigitsDivisor[_digits];


            int authenticationCode = Math.Abs(code);//轉換為正整數

            // pad with leading zeroes
            m_authenticationCode = authenticationCode.ToString().PadLeft(_digits, '0');
            m_expiry = GetExpiry(utcNow);
        }




        private static readonly int[] DigitsDivisor = new int[] { 0, 0, 0, 0, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };
        private static long ToUnixTime(DateTime dateTime) //轉成UnixTime utc轉 時間戳記
        {
            double unixSeconds = (dateTime - m_UnixTime).TotalSeconds;
            return Convert.ToInt64(Math.Round(unixSeconds));
        }
        private static DateTime GetExpiry(DateTime generationTimeUtc) //取得到期時間
        {
            long unixNow = ToUnixTime(generationTimeUtc);
            long secondsToExpiry = Effective_Time - unixNow % Effective_Time;
            DateTime expiry = generationTimeUtc + TimeSpan.FromSeconds(secondsToExpiry);

            if (expiry.Second % Effective_Time == 0)
            {
                // 將間隔的最後一秒標記為過期秒
                expiry = expiry - TimeSpan.FromSeconds(1);
            }

            // 設定到最小的單位 毫秒
            expiry = new DateTime(expiry.Year, expiry.Month, expiry.Day, expiry.Hour, expiry.Minute, expiry.Second, 999);
            return expiry;
        }

        public override string ToString()
        {
            string expiry = m_expiry.ToString("yyyy/MM/dd HH:mm:ss.fff");
            return m_authenticationCode; // "Authentication code: " + m_authenticationCode + " (expires at " + expiry + " UTC)"; // 
        }
    }
    public class Base32
    {
        public static byte[] ToByteArray(string input)
        {
            input = input.TrimEnd('=');
            int numBytes = input.Length * 5 / 8;
            byte[] result = new byte[numBytes];

            byte curByte = 0, bitsRemaining = 8;
            int mask = 0;
            int arrayIndex = 0;

            foreach (char c in input)//位移用
            {
                int ascii = CharToInt(c);

                if (bitsRemaining > 5)
                {
                    mask = ascii << (bitsRemaining - 5);
                    curByte = (byte)(curByte | mask);
                    bitsRemaining -= 5;
                }
                else
                {
                    mask = ascii >> (5 - bitsRemaining);
                    curByte = (byte)(curByte | mask);
                    result[arrayIndex++] = curByte;
                    curByte = (byte)(ascii << (3 + bitsRemaining));
                    bitsRemaining += 3;
                }
            }

            if (arrayIndex != numBytes)
            {
                result[arrayIndex] = curByte;
            }

            return result;
        }
        private static readonly System.Collections.Generic.List<char> base32Alphabet = new System.Collections.Generic.List<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-");
        private static int CharToInt(char c)
        {
            int ascii = c;

            if (base32Alphabet.IndexOf(char.ToUpperInvariant(c)) != -1)
            {
                return base32Alphabet.IndexOf(char.ToUpperInvariant(c));
            }
            throw new ArgumentException(string.Format("Invalid base32 character {0}", c));
        }

    }
}