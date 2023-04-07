using System;
using System.Net.Http;
using System.Threading.Tasks;
using CmlLib.Core.Auth.Microsoft.XboxAuthStrategies;
using CmlLib.Core.Auth.Microsoft.SessionStorages;
using CmlLib.Core.Auth.Microsoft.Executors;

namespace CmlLib.Core.Auth.Microsoft.Builders
{
    public abstract class XboxGameAuthenticationBuilder<T>
        where T : XboxGameAuthenticationBuilder<T>
    {
        protected Func<T, IXboxAuthStrategy>? XboxAuthStrategyFactory;

        private ISessionStorage? _sessionStorage;
        public ISessionStorage SessionStorage
        {
            get => _sessionStorage ??= new InMemorySessionStorage();
            set => _sessionStorage = value;
        }

        private HttpClient? _httpClient;
        public HttpClient HttpClient
        {
            get => _httpClient ??= HttpHelper.DefaultHttpClient.Value;
            set => _httpClient = value;
        }

        public T WithXboxAuth(IXboxAuthStrategy xboxAuthStrategy)
        {
            this.XboxAuthStrategyFactory = (_ => xboxAuthStrategy);
            return GetThis();
        }

        public T WithXboxAuth(Func<T, IXboxAuthStrategy> factory)
        {
           this.XboxAuthStrategyFactory = factory;
           return GetThis();
        }

        public T WithSessionStorage(ISessionStorage sessionStorage)
        {
            this.SessionStorage = sessionStorage;
            return GetThis();
        }

        public T WithHttpClient(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
            return GetThis();
        }

        protected virtual T GetThis()
        {
            return (T)this;
        }

        public abstract IAuthenticationExecutor Build();

        public Task<ISession> ExecuteAsync()
        {
            return Build().ExecuteAsync();
        }
    }
}