using System.Net.Http;
using CmlLib.Core.Auth.Microsoft.XboxAuthStrategies;
using CmlLib.Core.Auth.Microsoft.SessionStorages;
using CmlLib.Core.Auth.Microsoft.OAuthStrategies;
using CmlLib.Core.Auth.Microsoft.Builders;

namespace CmlLib.Core.Auth.Microsoft.MsalClient
{
    public class MsalXboxBuilder
    {
        public HttpClient? HttpClient { get; set; }
        public ISessionStorage? SessionStorage { get; set; }
        public MsalOAuthStrategyBuilder<MsalXboxBuilder> MsalOAuth { get; private set; }
        public XboxAuthStrategyBuilder<MsalXboxBuilder> XboxAuth { get; private set; }

        public MsalXboxBuilder()
        {
            MsalOAuth = createMsalBuilder();
            XboxAuth = createXboxAuthBuilder();
        }

        private MsalOAuthStrategyBuilder<MsalXboxBuilder> createMsalBuilder()
        {
            var builder = new MsalOAuthStrategyBuilder<MsalXboxBuilder>(this);
            return builder;
        }

        private XboxAuthStrategyBuilder<MsalXboxBuilder> createXboxAuthBuilder()
        {
            var builder = new XboxAuthStrategyBuilder<MsalXboxBuilder>(this, getHttpClient());
            return builder;
        }

        public MsalXboxBuilder WithXboxGameAuthenticationBuilder<T>(XboxGameAuthenticationBuilder<T> builder)
            where T : XboxGameAuthenticationBuilder<T>
        {
            if (builder.SessionStorage != null)
                WithSessionStorage(builder.SessionStorage);
            if (builder.HttpClient != null)
                WithHttpClient(builder.HttpClient);

            return this;
        }

        public MsalXboxBuilder WithSessionStorage(ISessionStorage sessionStorage)
        {
            this.SessionStorage = sessionStorage;
            return this;
        }

        public MsalXboxBuilder WithHttpClient(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
            return this;
        }

        private HttpClient getHttpClient() => HttpClient ??= new HttpClient();

        public IXboxAuthStrategy Build()
        {
            // MicrosoftOAuth
            // provide default SessionSource using _parentBuilder's SessionStorage
            if (MsalOAuth.SessionSource == null && this.SessionStorage != null)
                MsalOAuth.WithSessionSource(new MicrosoftOAuthSessionSource(this.SessionStorage));
            var oAuthStrategy = MsalOAuth.Build();

            // XboxAuth
            XboxAuth.WithMicrosoftOAuthStrategy(oAuthStrategy);
            if (XboxAuth.SessionSource == null && this.SessionStorage != null)
                XboxAuth.WithSessionSource(new XboxSessionSource(this.SessionStorage));
            return XboxAuth.Build();
        }
    }
}