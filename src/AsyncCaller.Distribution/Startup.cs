﻿using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using Nexus.Link.Configurations.Sdk;
using Nexus.Link.Libraries.Core.Application;
using Nexus.Link.Libraries.Core.MultiTenant.Model;
using Nexus.Link.Libraries.Core.Platform.Authentication;
using Nexus.Link.Libraries.Core.Platform.Configurations;
using Nexus.Link.Logger.Sdk;
using Nexus.Link.Logger.Sdk.RestClients;
using FulcrumApplicationHelper = Nexus.Link.Libraries.Web.AspNet.Application.FulcrumApplicationHelper;

[assembly: FunctionsStartup(typeof(AsyncCaller.Distribution.Startup))]

namespace AsyncCaller.Distribution
{
    public class Startup : FunctionsStartup
    {
        public static NexusSettings NexusSettings { get; } = new NexusSettings();
        public static ILeverServiceConfiguration AsyncCallerServiceConfiguration { get; set; }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables().Build();
            config.Bind("Nexus", NexusSettings);

            FulcrumApplicationHelper.WebBasicSetup($"async-caller-function-app-{NexusSettings.ServiceTenant.Organization}-{NexusSettings.ServiceTenant.Environment}", NexusSettings.ServiceTenant, NexusSettings.RuntimeLevel);

            var nexusServiceCredentials = new AuthenticationCredentials { ClientId = NexusSettings.Authentication.ClientId, ClientSecret = NexusSettings.Authentication.ClientSecret };
            var loggingConfiguration = new LeverServiceConfiguration(NexusSettings.ServiceTenant, "logging", NexusSettings.FundamentalsUrl, nexusServiceCredentials, NexusSettings.FundamentalsUrl);
            var logClient = new LogClient("http://this.will.be.ignored", new BasicAuthenticationCredentials());
            var logger = new FulcrumLogger(logClient, loggingConfiguration);
            FulcrumApplication.Setup.SynchronousFastLogger = logger;

            AsyncCallerServiceConfiguration = new LeverServiceConfiguration(NexusSettings.ServiceTenant, "AsyncCaller", NexusSettings.FundamentalsUrl, nexusServiceCredentials, NexusSettings.FundamentalsUrl);
        }
    }

    public class NexusSettings
    {
        public string Organization { get; set; }
        public string Environment { get; set; }
        public Tenant ServiceTenant => new Tenant(Organization, Environment);
        public RunTimeLevelEnum RuntimeLevel { get; set; }
        public string FundamentalsUrl { get; set; }
        public Authentication Authentication { get; set; }
    }

    public class Authentication
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}