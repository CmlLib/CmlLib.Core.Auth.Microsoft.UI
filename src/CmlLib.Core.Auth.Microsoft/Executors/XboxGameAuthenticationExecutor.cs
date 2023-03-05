using System.Threading.Tasks;
using CmlLib.Core.Auth.Microsoft.XboxAuthStrategies;
using CmlLib.Core.Auth.Microsoft.XboxGame;

namespace CmlLib.Core.Auth.Microsoft.Executors
{
    public class XboxGameAuthenticationExecutor<T> : IAuthenticationExecutor where T : ISession
    {
        private readonly IXboxGameAuthenticator<T> _gameAuthenticator;
        private readonly IXboxAuthStrategy _xboxAuthStrategy;

        public XboxGameAuthenticationExecutor(
            IXboxAuthStrategy xboxAuthStrategy, 
            IXboxGameAuthenticator<T> gameAuthenticator)
        {
            this._gameAuthenticator = gameAuthenticator;
            this._xboxAuthStrategy = xboxAuthStrategy;
        }

        public async Task<ISession> ExecuteAsync()
        {
            var result = await _gameAuthenticator.Authenticate(_xboxAuthStrategy);
            return (T)result;
        }
    }
}