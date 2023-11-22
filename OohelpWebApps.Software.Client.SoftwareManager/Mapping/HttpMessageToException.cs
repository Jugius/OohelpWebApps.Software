using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SoftwareManager.Mapping;
internal static class HttpMessageToException
{
    public static async Task<Exception> ToHttpException(this HttpResponseMessage message, string errorType = null)
    {
        using (message)
        {
            string error = errorType == null 
                ? $"StatusCode: {message.StatusCode}. Reason: {message.ReasonPhrase}."
                : $"{errorType}. StatusCode: {message.StatusCode}. Reason: {message.ReasonPhrase}.";
            string errorMessage = await message.Content.ReadAsStringAsync();
            if (errorMessage != null) error += $" Message: {errorMessage}";
            return new Exception(error) ;
        }        
    }
}
