using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;

namespace Acme.UI.Infrastructure.Routing
{
    public class RouteParameters
    {
        public Type DataSourceType { get; private set; }

        public string SearchNodeByName { get; private set; }

        public string RootNodeName { get; private set; }

        public RouteParameters(Type dataSourceType, string searchNodeByName, string rootNodeName)
        {
            DataSourceType = dataSourceType;
            SearchNodeByName = searchNodeByName;
            RootNodeName = rootNodeName;
        }
    }
}