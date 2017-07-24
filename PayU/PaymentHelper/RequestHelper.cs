using Newtonsoft.Json;
using PayU.Core;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace PayU.PaymentHelper
{
    public class RequestHelper<T> where T : new()
    {
        public T SendRequest(string Endpoint, NameValueCollection data)
        {
            var webClient = new WebClient();

            try
            {
                var response = Encoding.UTF8.GetString(webClient.UploadValues(Endpoint, data));
                return ParseResponse(response);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                }
                throw new PayuException("Send Request Error", ex);
            }
            catch (Exception ex)
            {
                throw new PayuException("Send Request Error", ex);
            }
        }
        static T ParseResponse(string stringResponse)
        {

            var result = JsonConvert.DeserializeObject<T>(stringResponse);
            return result;
        }
    }
}