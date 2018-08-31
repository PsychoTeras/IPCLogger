using System;
using System.Collections.Generic;
using Nancy.Security;

namespace IPCLogger.ConfigurationService.Entities
{
    internal class User : IUserIdentity
    {
        public string UserName { get; set; }

        public IEnumerable<string> Claims => null;
    }
}
