namespace RFID.REST.Common
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using RFID.REST.Middleware;
    using Serilog;
    using Serilog.Events;
    using Serilog.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public static class Extensions
    {
        private static readonly String outputTemplate = "[{Level}] {NewLine}[{Timestamp:o}] {NewLine}[Request Id: {RequestId}] {NewLine}[Http {RequestMethod}: {RequestPath}] {NewLine}[Request Query String: {RequestQueryString}] {NewLine}[Request Content-Type: {RequestContentType}] {NewLine}[Request Accept: {RequestAccept}] {NewLine}[Respose Content-Type: {ResposeContentType}] {NewLine}[User: {Username}] {NewLine}[Request Body: {RequestBody}] {NewLine}{Message} {NewLine}{Exception}{NewLine}";

        /// <summary>
        /// Returns Email claim or null
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public static String Email(this ClaimsPrincipal cp)
        {
            return cp.FindFirstValue(ClaimTypes.Name);
        }

        public static IWebHostBuilder UseSerilogLogging(this IWebHostBuilder builder)
        {
            return builder.UseSerilog();
        }

        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder appBuilder)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("RFID", LogEventLevel.Information)
                //.MinimumLevel.Verbose()
                //.MinimumLevel.Override("System", LogEventLevel.Verbose)
                //.MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                //.MinimumLevel.Override("RFID", LogEventLevel.Verbose)
                .WriteTo.Async(x => x.RollingFile(pathFormat: "Logs\\errors-{Date}.log", fileSizeLimitBytes: 1_000_000, outputTemplate: outputTemplate))
                .WriteTo.Async(x => x.Console())
                .Enrich.WithDemystifiedStackTraces()
                .Enrich.FromLogContext()
             .CreateLogger();

            Log.Logger = logger;

            return appBuilder
                .UseMiddleware<ExceptionMiddleware>()
                .UseSerilogRequestLogging();
        }
    }
}
