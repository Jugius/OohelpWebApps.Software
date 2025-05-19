using System.Collections.ObjectModel;
using System.Data;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace OohelpWebApps.Software.Server;

internal static class SerilogExtentions
{
    public static void RegisterSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithUtcTime()
                .Enrich.WithUserInfo()
                .WriteTo.Async(wt =>
                    wt.Console(
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))
                .WriteToSqlServer(hostBuilderContext.Configuration, builder)
         );
    }
    private static void WriteToSqlServer(this LoggerConfiguration serilogConfig, IConfiguration configuration, WebApplicationBuilder builder)
    {
       if (builder.Environment.IsDevelopment()) return;

        var connectionString =
               configuration.GetConnectionString("LogsDatabaseConnection");

        if (string.IsNullOrEmpty(connectionString)) return;

        MSSqlServerSinkOptions sinkOpts = new()
        {
            TableName = "SystemLogs",
            SchemaName = "dbo",
            AutoCreateSqlDatabase = false,
            AutoCreateSqlTable = false,
            BatchPostingLimit = 100,
            BatchPeriod = new TimeSpan(0, 0, 20),
        };

        ColumnOptions columnOpts = new()
        {
            Store = new Collection<StandardColumn>
            {
                StandardColumn.Id,
                StandardColumn.TimeStamp,
                StandardColumn.Level,
                StandardColumn.LogEvent,
                StandardColumn.Exception,
                StandardColumn.Message,
                StandardColumn.MessageTemplate,
            },
            AdditionalColumns = new Collection<SqlColumn>
            {
                new()
                {
                    ColumnName = "ClientIP", PropertyName = "ClientIP",AllowNull=true, DataType = SqlDbType.NVarChar, DataLength = 64
                },
                new()
                {
                    ColumnName = "UserName", PropertyName = "UserName",AllowNull=true, DataType = SqlDbType.NVarChar
                },
                new()
                {
                    ColumnName = "ClientAgent", PropertyName = "ClientAgent",AllowNull=true, DataType = SqlDbType.NVarChar
                },
                new()
                {
                    ColumnName = "Application", PropertyName = "Application", AllowNull=false, DataType = SqlDbType.NVarChar, DataLength = 64
                }
            },
            TimeStamp = { ConvertToUtc = true, ColumnName = "TimeStamp" },
            LogEvent = { DataLength = -1 }
        };
        columnOpts.PrimaryKey = columnOpts.Id;
        columnOpts.TimeStamp.NonClusteredIndex = true;

        serilogConfig.WriteTo.Async(wt => wt.MSSqlServer(
            connectionString,
            sinkOpts,
            columnOptions: columnOpts
        ));
    }
    private static LoggerConfiguration WithUtcTime(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        return enrichmentConfiguration.With<UtcTimestampEnricher>();
    }
    private static LoggerConfiguration WithUserInfo(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        return enrichmentConfiguration.With<UserInfoEnricher>();
    }
    internal class UtcTimestampEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf)
        {
            logEvent.AddOrUpdateProperty(pf.CreateProperty("TimeStamp", logEvent.Timestamp.UtcDateTime));
        }
    }
    internal class UserInfoEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserInfoEnricher() : this(new HttpContextAccessor())
        {
        }
        //Dependency injection can be used to retrieve any service required to get a user or any data.
        //Here, I easily get data from HTTPContext
        public UserInfoEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? string.Empty;
            var headers = _httpContextAccessor.HttpContext?.Request?.Headers;
            var clientIp = headers != null && headers.TryGetValue("X-Forwarded-For", out var value)
            ? value.ToString().Split(',').First().Trim()
            : _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;
            var clientAgent = headers != null && headers.TryGetValue("User-Agent", out value)
                ? value.ToString()
                : string.Empty;

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", userName));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientIP", clientIp));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientAgent", clientAgent));
        }
    }
}
