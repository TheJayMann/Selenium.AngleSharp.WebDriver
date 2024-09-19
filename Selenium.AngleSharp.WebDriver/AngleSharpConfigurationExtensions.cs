using Selenium.AngleSharp.WebDriver;

namespace AngleSharp {
    public static class ConfigurationExtensions {
        public static IConfiguration WithHistory(this IConfiguration configuration) => 
            configuration.With(ctx => new AngleSharpHistory(ctx))
        ;
    }
}
