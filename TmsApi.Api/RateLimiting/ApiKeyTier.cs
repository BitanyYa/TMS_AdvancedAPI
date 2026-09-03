namespace TmsApi.Api.RateLimiting;

public enum ApiKeyTier { Anonymous, Free, Paid }

public static class ApiKeyResolver
{
    public static (string PartitionKey, ApiKeyTier Tier) Resolve(HttpContext ctx)
    {
        var key = ctx.Request.Headers["X-Api-Key"].ToString();
        if (string.IsNullOrEmpty(key))
        {
            return (ctx.Connection.RemoteIpAddress?.ToString() ?? "anonymous", ApiKeyTier.Anonymous);
        }

        var configuration = ctx.RequestServices.GetRequiredService<IConfiguration>();
        var freeKey = configuration["TmsApi:FreeApiKey"]
            ?? Environment.GetEnvironmentVariable("TMS_FREE_API_KEY");
        var paidKey = configuration["TmsApi:PaidApiKey"]
            ?? Environment.GetEnvironmentVariable("TMS_PAID_API_KEY");

        if (string.Equals(key, paidKey, StringComparison.Ordinal) && !string.IsNullOrEmpty(paidKey))
            return (key, ApiKeyTier.Paid);

        if (string.Equals(key, freeKey, StringComparison.Ordinal) && !string.IsNullOrEmpty(freeKey))
            return (key, ApiKeyTier.Free);

        return (key, ApiKeyTier.Anonymous);
    }
}