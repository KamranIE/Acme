using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Acme.UI.Helper.Services
{
    public class CustomService2 : IAcmeService<string, string>
    {
        public string Process(string arg)
        {
            return $" - Processed2: {arg}";
        }
    }
}