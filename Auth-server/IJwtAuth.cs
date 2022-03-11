using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth_server
{
    public interface IJwtAuth
    {
        string Authentication(string username, string password);
    }
}
