using System.Text;
using Microsoft.AspNetCore.Http;

namespace Mwm.ReSync.ServerEvents.Extensions;

public static class HttpExtensions {

    public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding? encoding = null) {
        if (encoding == null)
            encoding = Encoding.UTF8;

        using StreamReader reader = new StreamReader(request.Body, encoding);
        return await reader.ReadToEndAsync();
    }

}

