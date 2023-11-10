namespace Payment.App.Common;

public class EncryptionHelper
{
    public static string Encrypt(string param1, string param2)
    {
        string plainText = $"{param1}|{param2}";
        byte[] encryptedBytes;

        using (TripleDES tripleDES = TripleDES.Create())
        {
            tripleDES.Key = GenerateRandomKey(tripleDES.KeySize / 8);
            tripleDES.IV = GenerateRandomIV(tripleDES.BlockSize / 8);

            ICryptoTransform encryptor = tripleDES.CreateEncryptor(tripleDES.Key, tripleDES.IV);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        return Convert.ToBase64String(encryptedBytes);
    }

    public static (string, string) Decrypt(string encryptedText)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        string plainText;

        using (TripleDES tripleDES = TripleDES.Create())
        {
            tripleDES.Key = GenerateRandomKey(tripleDES.KeySize / 8);
            tripleDES.IV = GenerateRandomIV(tripleDES.BlockSize / 8);

            ICryptoTransform decryptor = tripleDES.CreateDecryptor(tripleDES.Key, tripleDES.IV);

            byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            plainText = Encoding.UTF8.GetString(decryptedBytes);
        }

        string[] paramsArray = plainText.Split('|');

        if (paramsArray.Length == 2)
        {
            return (paramsArray[0], paramsArray[1]);
        }
        else
        {
            throw new ArgumentException("Invalid encrypted text");
        }
    }

    private static byte[] GenerateRandomKey(int length)
    {
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] key = new byte[length];
            rng.GetBytes(key);
            return key;
        }
    }

    private static byte[] GenerateRandomIV(int length)
    {
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] iv = new byte[length];
            rng.GetBytes(iv);
            return iv;
        }
    }
}