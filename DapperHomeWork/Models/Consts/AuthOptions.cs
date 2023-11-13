using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DapperHomeWork.Models.Consts;

public class AuthOptions
{
    public const string ISSUER = "DapperHomeWork";
    public const string AUDIENCE = "DapperHomeWorkClient";
    public const int LIFETIME = 1;

    private const string KEY = "super_secret_key_by_me";

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}

