using System;
using System.Security.Cryptography;
using System.Text;

public class FairRandomGenerator
{
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    public (int Number, string Hmac, byte[] Key) Generate(int min, int max)
    {
        byte[] key = new byte[32];
        _rng.GetBytes(key);

        int number = GenerateSecureRandom(min, max);

        using var hmac = new HMACSHA256(key);
        string hmacValue = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(number.ToString()))).Replace("-", "");

        return (number, hmacValue, key);
    }

    private int GenerateSecureRandom(int min, int max)
    {
        if (min >= max) throw new ArgumentException("Invalid range.");

        int range = max - min;
        byte[] buffer = new byte[4];
        _rng.GetBytes(buffer);
        int result = BitConverter.ToInt32(buffer, 0) & int.MaxValue;

        return min + (result % range);
    }


    
    static byte[] ConvertHexStringToBytes(string hex)
    {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("Hex string must have an even number of characters.");

        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < hex.Length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    
    static string GetOriginalHMAC(byte[] key, int number){
        // int number = 0;
        // byte[] key = ConvertHexStringToBytes("4A331507C381D9566166408CB9D1B3B09EAD9753B487C8FB826F82A08B9C9B2B"); 
        using var hmacSha256 = new HMACSHA256(key);
        string computedHmac = BitConverter.ToString(hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(number.ToString()))).Replace("-", "");
       
        // 67353A60988D9D6CBF78F895EDFCF7C305762E6F3E6ECD65D746C8772EDD5B1E
        return computedHmac;
    }

}
