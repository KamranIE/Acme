using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Acme.UI.Infrastructure.Routing
{
    public class RouteParser
    {
        private const char _backslashSeparator = '\\';
        private const char _forwardlashSeparator = '/';
        private const char _questionMarkSeparator = '?';
        private const char _hyphenSeparator = '-';
        private const string _startBracket = "{";
        private const string _endBracket = "}";
        private readonly bool _hasParameters;
        private Dictionary<string, string> _tokensAndValues;


        public bool IsMapped { get; private set; }

        public RouteParser(string route, string incomingPath)
        {
            var routeTokens = ParseToTokens(route);
            var pathTokens = ParseToTokens(incomingPath);

            _tokensAndValues = MapTokens(routeTokens, pathTokens);
            IsMapped = CheckIfMapped(_tokensAndValues);
        }

        public bool HasParameters
        {
            get
            {
                if (_tokensAndValues != null)
                {
                    return _tokensAndValues.Keys.Any(key => IsParameter(key));
                }
                return false;
            }
        }

        public string GetParameterValue(string parameterName)
        {
            if (_tokensAndValues != null && _tokensAndValues.Keys.Contains(parameterName))
            {
                return _tokensAndValues[parameterName];
            }
            return null;
        }

        private List<string> ParseToTokens(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var pathOnly = value.Split(new char[] { _questionMarkSeparator }, StringSplitOptions.RemoveEmptyEntries).First(); // separate parameters if any

                return pathOnly.Split(new char[] { _backslashSeparator, _forwardlashSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            return new List<string> { { value } };
        }

        private Dictionary<string, string> MapTokens(List<string> routeTokens, List<string> pathTokens)
        {
            var result = new Dictionary<string, string>();
            var pathTokensCount = pathTokens != null ? pathTokens.Count : 0;
            for (int i = 0; i < routeTokens.Count; i++)
            {
                result.Add(routeTokens[i], pathTokensCount > i ? pathTokens[i] : null);
            }

            return result;
        }

        private bool CheckIfMapped(Dictionary<string, string> tokensAndValues)
        {
            foreach (var pair in tokensAndValues)
            {
                if (!IsParameter(pair.Key)) // if current route segment is not a parameter e.g. dashboard
                {
                    if (string.Compare(pair.Key, pair.Value, true) != 0) // then both route and path segments must match i.e. route:...\dashboard\... ;     path:...\dashboard\...
                    {
                        return false; // else it is not mapped
                    }
                }
                else // if current route segment is a parameter e.g. {athleteName}
                {
                    if (pair.Value == null)  // then  there should be some value for that parameter. Right now there is no optional route value
                    {
                        return false;
                    }
                }
            }

            return true; // if above loop passed, it is a map
        }

        private bool IsParameter(string value)
        {
            return value != null && value.StartsWith(_startBracket) && value.EndsWith(_endBracket);
        }
    }


    public class RouteParser2
    {
        private const string patternRoute = "(?<root>[a-zA-Z0-9-]+)/(?<section>[a-zA-Z0-9-]+)/(?<param>[a-zA-Z0-9-{}]+)";
        
        public RouteParser2(string route, string incomingPath)
        {
            /*
            var routeTokens = ParseToTokens(route);
            var pathTokens = ParseToTokens(incomingPath);

            _tokensAndValues = MapTokens(routeTokens, pathTokens);
            IsMapped = CheckIfMapped(_tokensAndValues);
            */
        }
        /*
        public List<string> Parse(string value, string pattern)
        {
            Regex regex = new Regex(pattern);
            var matches = regex.Matches(value);

            foreach(var match in matches)
            {
                
            }
        }
        */

    }
}