using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Acme.UI.Helper.Services
{
    public class CustomService : IAcmeService<string, string>
    {
        public string Process(string arg)
        {
            return $" - Processed: {arg}";
        }
    }
}