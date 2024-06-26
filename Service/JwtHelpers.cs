using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Net.NetworkInformation;

public class JwtHelpers
{
    private readonly IConfiguration Configuration;

    public JwtHelpers(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }
    // public string GenerateGuestToken(string guestId, string roomPinCode, string guestName, int expireHours = 5)
    // {
    //     var signKey = Configuration.GetValue<string>("JwtSettings:GuestSignKey");

    //     if (string.IsNullOrEmpty(signKey))
    //     {
    //         throw new ArgumentNullException(nameof(signKey), "GuestSignKey cannot be null or empty.");
    //     }

    //     var claims = new List<Claim>
    //     {
    //         new Claim("guestId", guestId),
    //         new Claim("roomPinCode", roomPinCode),
    //         new Claim(JwtRegisteredClaimNames.Sub, guestName),
    //         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    //     };

    //     var guestClaimsIdentity = new ClaimsIdentity(claims);

    //     var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));
    //     var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

    //     var tokenDescriptor = new SecurityTokenDescriptor
    //     {
    //         Subject = guestClaimsIdentity,
    //         Expires = DateTime.UtcNow.AddHours(expireHours),
    //         SigningCredentials = signingCredentials
    //     };

    //     var tokenHandler = new JwtSecurityTokenHandler();
    //     var securityToken = tokenHandler.CreateToken(tokenDescriptor);
    //     var serializedToken = tokenHandler.WriteToken(securityToken);

    //     return serializedToken;
    // }


    public string GenerateToken(string userName,int Role, int expireHours = 5)
    {
        var issuer = Configuration.GetValue<string>("JwtSettings:Issuer");
        var signKey = Configuration.GetValue<string>("JwtSettings:UserSignKey");

        // Configuring "Claims" to your JWT Token
        var claims = new List<Claim>
        {
            // In RFC 7519 (Section#4), there are defined 7 built-in Claims, but we mostly use 2 of them.
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iss, issuer));
            new(JwtRegisteredClaimNames.Sub, userName), // User.Identity.Name
            //claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "The Audience"));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds().ToString()));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT ID
        };

        // The "NameId" claim is usually unnecessary.
        //claims.Add(new Claim(JwtRegisteredClaimNames.NameId, userName));

        // This Claim can be replaced by JwtRegisteredClaimNames.Sub, so it's redundant.
        //claims.Add(new Claim(ClaimTypes.Name, userName));
        
        // TODO: You can define your "roles" to your Claims.
        if(Role == 1)
            claims.Add(new Claim(ClaimTypes.Role, "Student"));
        else if(Role == 2)
            claims.Add(new Claim(ClaimTypes.Role, "Teacher"));
        else if(Role == 3)
            claims.Add(new Claim(ClaimTypes.Role, "Manager"));
        else if(Role == 4)
            claims.Add(new Claim(ClaimTypes.Role, "ForgetPassword"));
        else if(Role == 5)
            claims.Add(new Claim(ClaimTypes.Role, "Guest"));
        else if(Role == 6)
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        var userClaimsIdentity = new ClaimsIdentity(claims);

        // Create a SymmetricSecurityKey for JWT Token signatures
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

        // HmacSha256 MUST be larger than 128 bits, so the key can't be too short. At least 16 and more characters.
        // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        // Create SecurityTokenDescriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            //Audience = issuer, // Sometimes you don't have to define Audience.
            //NotBefore = DateTime.Now, // Default is DateTime.Now
            //IssuedAt = DateTime.Now, // Default is DateTime.Now
            Subject = userClaimsIdentity,
            Expires = DateTime.Now.AddHours(expireHours),
            SigningCredentials = signingCredentials
        };

        // Generate a JWT securityToken, than get the serialized Token result (string)
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var serializeToken = tokenHandler.WriteToken(securityToken);

        return serializeToken;
    }
    public string GenerateToken(string userName,int Role,string roomPinCode, int expireHours = 5)
    {
        var issuer = Configuration.GetValue<string>("JwtSettings:Issuer");
        var signKey = Configuration.GetValue<string>("JwtSettings:UserSignKey");

        // Configuring "Claims" to your JWT Token
        var claims = new List<Claim>
        {
            // In RFC 7519 (Section#4), there are defined 7 built-in Claims, but we mostly use 2 of them.
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iss, issuer));
            new(JwtRegisteredClaimNames.Sub, userName), // User.Identity.Name
            //claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "The Audience"));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds().ToString()));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
            // The "NameId" claim is usually unnecessary.
            //claims.Add(new Claim(JwtRegisteredClaimNames.NameId, userName));

            // This Claim can be replaced by JwtRegisteredClaimNames.Sub, so it's redundant.
            //claims.Add(new Claim(ClaimTypes.Name, userName));

            new("roomPinCode", roomPinCode)
        };

        // TODO: You can define your "roles" to your Claims.
        if(Role == 1)
            claims.Add(new Claim(ClaimTypes.Role, "Student"));
        else if(Role == 2)
            claims.Add(new Claim(ClaimTypes.Role, "Teacher"));
        else if(Role == 3)
            claims.Add(new Claim(ClaimTypes.Role, "Manager"));
        else if(Role == 4)
            claims.Add(new Claim(ClaimTypes.Role, "ForgetPassword"));
        else if(Role == 5)
            claims.Add(new Claim(ClaimTypes.Role, "Guest"));
        else if(Role == 6)
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        var userClaimsIdentity = new ClaimsIdentity(claims);

        // Create a SymmetricSecurityKey for JWT Token signatures
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

        // HmacSha256 MUST be larger than 128 bits, so the key can't be too short. At least 16 and more characters.
        // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        // Create SecurityTokenDescriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            //Audience = issuer, // Sometimes you don't have to define Audience.
            //NotBefore = DateTime.Now, // Default is DateTime.Now
            //IssuedAt = DateTime.Now, // Default is DateTime.Now
            Subject = userClaimsIdentity,
            Expires = DateTime.Now.AddHours(expireHours),
            SigningCredentials = signingCredentials
        };

        // Generate a JWT securityToken, than get the serialized Token result (string)
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var serializeToken = tokenHandler.WriteToken(securityToken);

        return serializeToken;
    }
}