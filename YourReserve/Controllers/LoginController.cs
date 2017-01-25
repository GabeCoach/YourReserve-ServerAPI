/* Description: This controller handles all login functanality for the server
 * Methods: LoginCustomer, LoginRestaurant
 * Author: Gabriel Coach 
 * Email: gsctca@gmail.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.IdentityModel.Tokens;

using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using YourReserve.Models;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;

using Newtonsoft.Json;

using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel;

namespace YourReserve.Controllers
{
    [Authorize]
    public class LoginController : ApiController
    {
        string sBaseURL = "http://localhost/YourReserve";
        string sClaimLifeTime = System.Configuration.ConfigurationManager.AppSettings["ClaimLife"];

        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();
        private EncryptDecrypt oCrypt = new EncryptDecrypt();

        /* Description: Perform Login request for the YourReserve Customer website.
         * Return Value: The JWT Token string is returned from this function.
        */
        [HttpPost]
        [AllowAnonymous]
        [Route("api/LoginController/LoginCustomer")]
        public HttpResponseMessage LoginCustomer()
        {
            //Get Request IPAddress
            string sIPAddress = Request.GetOwinContext().Request.RemoteIpAddress;

            //Retrieve the Request body content and deserialise to the UserModel Object
            Task<string> logindata = Request.Content.ReadAsStringAsync();
            string sData = logindata.Result;
            UserModel oLoginData = JsonConvert.DeserializeObject<UserModel>(sData);

            //Encrypt the user password to match password in database
            var oDecrypt = new EncryptDecrypt();
            oDecrypt.HashValue = "13yH5J87m3Xx8";
            oDecrypt.SaltKey = "Gh7f8JAs308f6";
            oDecrypt.VIKey = "Hj74K45yGn28A51l";
            string sEncryptedUserName = oLoginData.UserName;
            string sEncryptedPassword = oDecrypt.Encrypt(oLoginData.Password);

            //Perform Login
            sEncryptedUserName = PerformLogin2(sEncryptedUserName, sEncryptedPassword);

            string sUserName = sEncryptedUserName;
            int iUserID = GetUserID(sUserName).First();

            //Return JWT Token if the PerformLogin function returns a Username.
            if (sUserName != "")
            {
                string sToken = GetJwtFromTokenIssuer(oLoginData.UserName);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<object>(new
                    {
                        Message = "Valid login.",
                        UserName = oLoginData.UserName,
                        AccessToken = sToken,
                        UserID = iUserID
                    }, Configuration.Formatters.JsonFormatter)
                };
            }

            //Return Invalid Login message.
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<object>(new
                {
                    Message = "Invalid login.",
                    UserName = "",
                    AccessToken = ""
                }, Configuration.Formatters.JsonFormatter)
            };
        }

        private IQueryable<int> GetUserID(string sUserName)
        {
            return from u in db.Users
                   where u.UserName == sUserName
                   select u.UserID;
        }

        /* Description: Perform Login for the Restaurant Dashboard Website
         * Follows same code, as LoginCustomer Method.
         */
        [HttpPost]
        [AllowAnonymous]
        [Route("api/LoginController/LoginRestaurant")]
        public HttpResponseMessage LoginRestaurant()
        {

            string sIPAddress = Request.GetOwinContext().Request.RemoteIpAddress;

            Task<string> logindata = Request.Content.ReadAsStringAsync();
            string sData = logindata.Result;

            UserModel oLoginData = JsonConvert.DeserializeObject<UserModel>(sData);

            var oDecrypt = new EncryptDecrypt();

            oDecrypt.HashValue = "13yH5J87m3Xx8";
            oDecrypt.SaltKey = "Gh7f8JAs308f6";
            oDecrypt.VIKey = "Hj74K45yGn28A51l";

            string sEncryptedUserName = oLoginData.UserName;
            string sEncryptedPassword = oDecrypt.Encrypt(oLoginData.Password);

            sEncryptedUserName = PerformLogin3(sEncryptedUserName, sEncryptedPassword);

            string sUserName = sEncryptedUserName;

            int RestaurantID = GetRestaurantID(sUserName);

            if (sUserName != "")
            {
                string sToken = GetJwtFromTokenIssuer(oLoginData.UserName);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<object>(new
                    {
                        Message = "Valid login.",
                        UserName = oLoginData.UserName,
                        AccessToken = sToken,
                        RestID = RestaurantID
                    }, Configuration.Formatters.JsonFormatter)
                };
            }

            return new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new ObjectContent<object>(new
                {
                    Message = "Invalid login.",
                    UserName = "",
                    AccessToken = ""
                }, Configuration.Formatters.JsonFormatter)
            };
        }

        private int GetRestaurantID(string UserName)
        {
            var query = (from r in db.RestaurantOwners
                        where r.UserName == UserName
                        select r.RestaurantID).FirstOrDefault();

            return Convert.ToInt32(query);
        }

        /* Description: Determine if username and password match stored database data
         * Params: Username, Password
         * Return: Username string.
        */
        private string PerformLogin2(string UserName, string Password)
        {
            var query =
                (from a in db.Users
                 where a.UserName == UserName && a.Password.Equals(Password)
                 select new
                 {
                     a.UserName,
                 }
            );

            string sResult = "";

            foreach (var x in query)
            {
                sResult = x.UserName;
            }

            return sResult;
        }

        private string PerformLogin3(string UserName, string Password)
        {
            try
            {
                var query =
               (from a in db.RestaurantOwners
                where a.UserName == UserName && a.Password.Equals(Password)
                select new
                {
                    a.UserName
                }
           );

                string sResult = "";

                foreach (var x in query)
                {
                    sResult = x.UserName;
                }

                return sResult;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }


        /* Description: Create JWT Token
         * Params: Username
         * Return: JWT Token string.
        */
        private string GetJwtFromTokenIssuer(string sUserName)
        {
            //Create token lifetime
            int iClaimLifeTime = 400;
            if (sClaimLifeTime != null && sClaimLifeTime != "")
            {
                iClaimLifeTime = Convert.ToInt32(sClaimLifeTime);
            }

            //Generate key
            KeySingleton keySingleton = KeySingleton.Instance();
            keySingleton.key = GenerateKey();
            string sKey = keySingleton.key;

            byte[] key = Convert.FromBase64String(sKey);

            var symmetericKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key);

            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(symmetericKey, SecurityAlgorithms.HmacSha256Signature);

            //Create new Token Descriptor object
            var descriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
            {
                Issuer = "http://authzserver.demo",
                Audience = sBaseURL + "/api",
                Expires = DateTime.Now.AddMinutes(iClaimLifeTime),
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, sUserName) })
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(descriptor);

            return tokenHandler.WriteToken(token);
        }

        private string GenerateKey()
        {
            var key = "";

            using (var provider = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] secretKeyBytes = new Byte[32];
                provider.GetBytes(secretKeyBytes);
                key = Convert.ToBase64String(secretKeyBytes);
            }

            return key;
        }
    }

    
    public class EncryptDecrypt
    {
        private static string _HashValue;
        private static string _SaltKey;
        private static string _VIKey;

       /* Description: Encrypt string value
        * Params: plainText
        * Return: Encrypted string value.
        */
        public string Encrypt(string plainText)
        {
            string sReturn = "";

            try
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                byte[] keyBytes = new System.Security.Cryptography.Rfc2898DeriveBytes(_HashValue, Encoding.ASCII.GetBytes(_SaltKey)).GetBytes(256 / 8);
                var symmetricKey = new System.Security.Cryptography.RijndaelManaged() { Mode = System.Security.Cryptography.CipherMode.CBC, Padding = System.Security.Cryptography.PaddingMode.Zeros };
                var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(_VIKey));

                byte[] cipherTextBytes;

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    using (var cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memoryStream.ToArray();
                        cryptoStream.Close();
                    }
                    memoryStream.Close();
                }
                sReturn = Convert.ToBase64String(cipherTextBytes);
            }
            catch (Exception ex)
            { }

            return sReturn;
        }

       /* Description: Decrypt string value
        * Params: plainText
        * Return: Encrypted string value.
        */
        public string Decrypt(string encryptedText)
        {
            string sReturn = "";

            try
            {
                byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
                byte[] keyBytes = new System.Security.Cryptography.Rfc2898DeriveBytes(_HashValue, Encoding.ASCII.GetBytes(_SaltKey)).GetBytes(256 / 8);
                var symmetricKey = new System.Security.Cryptography.RijndaelManaged() { Mode = System.Security.Cryptography.CipherMode.CBC, Padding = System.Security.Cryptography.PaddingMode.None };

                var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(_VIKey));
                var memoryStream = new System.IO.MemoryStream(cipherTextBytes);
                var cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream, decryptor, System.Security.Cryptography.CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                sReturn = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
            }
            catch (Exception ex)
            { }

            return sReturn;
        }


        public string HashValue
        {
            get
            {
                return _HashValue;
            }
            set
            {
                _HashValue = value;
            }
        }

        public string SaltKey
        {
            get
            {
                return _SaltKey;
            }
            set
            {
                _SaltKey = value;
            }
        }

        public string VIKey
        {
            get
            {
                return _VIKey;
            }
            set
            {
                _VIKey = value;
            }
        }
    }

    public class KeySingleton
    {
        private static KeySingleton _instance;

        protected KeySingleton()
        {

        }

        public static KeySingleton Instance()
        {
            if (_instance == null)
            {
                _instance = new KeySingleton();
            }

            return _instance;
        }

        public string key { get; set; }
    }

    public class UserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
