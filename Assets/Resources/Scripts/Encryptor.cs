using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;

public static class Encryptor
{
    private const string SEED = "Q!s@C#v$B%h^J&K*l(p).";

    public static string GetHash(string text)
    {
        var bytes = Encoding.ASCII.GetBytes(SEED + text);
        SHA256 shaM = new SHA256Managed();
        var encrypted = shaM.ComputeHash(bytes);
        var str = Convert.ToBase64String(encrypted);
        if (str.Length > 64) str = str[..64];
        return str;
    }

    public static string Sanitize(string text)
        => text.Replace("'", "''");
}
