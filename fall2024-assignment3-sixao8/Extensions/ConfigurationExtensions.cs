namespace fall2024_assignment3_sixao8.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetOrError<T>(this IConfiguration config, string key)
        {
            if (config[key] is null) throw new Exception($"'{key}' not in Configuration");
            return config.GetValue<T>(key)!;
        }
        public static string GetConnectionStringOrError(this IConfiguration config, string key)

        {
            return config.GetConnectionString(key) ?? throw new InvalidOperationException($"Connection string '{key}' not in Configuration.");
        }
        public static string GetOrError(this IConfiguration config, string key)
        {
            return config.GetOrError<string>(key);
        }
    }
}
