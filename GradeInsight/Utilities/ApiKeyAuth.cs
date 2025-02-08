using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace GradeInsight.Utilities
{
    public class ApiKeyAuth : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "Authorization";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey) || !context.HttpContext.Request.Headers.TryGetValue("APIRequestToc", out var potentialtime))
            {
                context.Result = new BadRequestResult();
                return;
            }
            else
            {
                //if (!(potentialApiKey == BasePageModel.APIKey))
                //{
                //    context.Result = new UnauthorizedResult();
                //    return;
                //
                //}
                var check = CheckAPIkey(potentialApiKey, potentialtime);
                if (!check)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
            await next();
        }
        public static bool CheckAPIkey(string Key, string requestdt)
        {
            var key = BasePageModel.APIKey;
            //var decodedkey = Key;
            var decodedkey = DecodeKey(Key, requestdt);
            if (Key != null && decodedkey !=null)
            {
                bool IsCorrect = Utilities.HashingAndVerification.Confirm(decodedkey, key, HashingAndVerification.Supported_HA.SHA256);
                return IsCorrect;
            }
            return false;
        }
        public static string? DecodeKey(string Key, string requestdt)
        {
            if (IsApiKeyValid(requestdt))
            {
                if (Key != null && Key.StartsWith("Bearer"))
                {
                    string encodedKey = Key.Substring("Bearer".Length).Trim();
                    Encoding encoding = Encoding.GetEncoding("UTF-8");
                    string decodedkey = encoding.GetString(Convert.FromBase64String(encodedKey));
                    int seperatorIndex = decodedkey.IndexOf(':');
                    var PassKey = decodedkey.Substring(0, seperatorIndex);
                    var Requesttime = decodedkey.Substring(seperatorIndex + 1);
                    return PassKey;
                }
                return null;
            }
            return null;
        }
        //check the time span of the apirequsetdt(use the local datetime check)
        //or
        //API key time check send in a random int that will be added with the utc time to check the validation of the key sent. same function as getapikey
        public static bool IsApiKeyValid(string requestdt)
        {
            DateTime start = DateTime.UtcNow.AddMinutes(-7);
            DateTime end = DateTime.UtcNow.AddMinutes(7);
            DateTime now = DateTime.Parse(requestdt);
            if (((now > start) && (now < end)))
            {
                return true;
            }
            return false;
        }
        //generate dummy api key to send through query param and which key type to check against (used while sending request not in admin) 
        //public string getapikey()
        //{
        //    string keytoencode = datetime.utcnow.tostring() + ":2";
        //    string returnkey = hashingandverification.encodeto64(keytoencode);
        //    return returnkey;
        //}
    }
}

