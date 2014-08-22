using System;
using System.Configuration;

namespace BusinessService
{
    public class BaseClass : IDisposable
    {
        protected readonly string Token = ConfigurationManager.AppSettings["token"];

        public void Dispose()
        {
            
        }
    }
}
