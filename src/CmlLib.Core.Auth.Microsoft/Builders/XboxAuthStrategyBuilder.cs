using System;
using System.Net.Http;
using CmlLib.Core.Auth.Microsoft.OAuthStrategies;
using CmlLib.Core.Auth.Microsoft.XboxAuthStrategies;
using CmlLib.Core.Auth.Microsoft.SessionStorages;

namespace CmlLib.Core.Auth.Microsoft.Builders
{
    public class XboxAuthStrategyBuilder<T> : MethodChaining<T>
    {
        private readonly HttpClient _httpClient;
        public ISessionSource<XboxAuthTokens>? SessionSource { get; set; }

        public bool UseCaching { get; set; } = true;
        private IMicrosoftOAuthStrategy? oAuthStrategy;
        private Func<IXboxAuthStrategy>? strategyGenerator;

        public XboxAuthStrategyBuilder(
            T returning,
            HttpClient httpClient) : base(returning)
        {
            _httpClient = httpClient;
        }

        public T WithMicrosoftOAuthStrategy(IMicrosoftOAuthStrategy strategy)
        {
            this.oAuthStrategy = strategy;
            return GetThis();
        }

        public T WithSessionSource(ISessionSource<XboxAuthTokens> sessionSource)
        {
            SessionSource = sessionSource;
            return GetThis();
        }

        public T WithCaching(bool useCaching)
        {
            UseCaching = useCaching;
            return GetThis();
        }

        public T UseBasicStrategy()
        {
            UseStrategy(() => 
            {
                if (oAuthStrategy == null)
                    throw new InvalidOperationException("this strategy require OAuthStrategy");
                var strategy = new BasicXboxAuthStrategy(_httpClient, oAuthStrategy);
                return withCachingIfRequired(strategy);
            });
            return GetThis();
        }

        public T UseStrategy(Func<IXboxAuthStrategy> strategy)
        {
            this.strategyGenerator = strategy;
            return GetThis();
        }

        public T FromXboxAuthTokens(XboxAuthTokens authTokens)
        {
            UseStrategy(() => 
            {
                var strategy = new XboxAuthStrategyFromTokens(authTokens);
                return withCachingIfRequired(strategy);
            });
            return GetThis();
        }

        public IXboxAuthStrategy CreateCachingStrategy(IXboxAuthStrategy strategy)
        {
            return new CachingXboxAuthStrategy(strategy, getOrCreateSessionSource());
        }

        private IXboxAuthStrategy withCachingIfRequired(IXboxAuthStrategy strategy)
        {
            if (UseCaching)
                return CreateCachingStrategy(strategy);
            else
                return strategy;
        }

        private ISessionSource<XboxAuthTokens> getOrCreateSessionSource()
        {
            if (SessionSource == null)
                return new InMemorySessionSource<XboxAuthTokens>();
            else
                return SessionSource;
        }

        public IXboxAuthStrategy Build()
        {
            if (strategyGenerator == null)
                throw new InvalidOperationException("set strategy");
            return strategyGenerator.Invoke();
        }
    }
}