using System.Threading.Tasks;
using CmlLib.Core.Auth.Microsoft.SessionStorages;
using CmlLib.Core.Auth.Microsoft.XboxAuthStrategies;

namespace CmlLib.Core.Auth.Microsoft.XboxGame
{
    public class SilentXboxGameAuthenticator<T> : IXboxGameAuthenticator<T> where T : ISession
    {
        private readonly IXboxGameAuthenticator<T> _innerAuthenticator;
        private readonly ISessionSource<T> _sessionSource;

        public SilentXboxGameAuthenticator(
            IXboxGameAuthenticator<T> authenticator,
            ISessionSource<T> sessionSource) =>
            (_innerAuthenticator, _sessionSource) = (authenticator, sessionSource);

        public async Task<T> Authenticate(IXboxAuthStrategy xboxAuthStrategy)
        {
            var storedSession = await _sessionSource.GetAsync();
            if (storedSession != null && storedSession.Validate())
                return storedSession;
            else
                return await _innerAuthenticator.Authenticate(xboxAuthStrategy);
        }
    }
}