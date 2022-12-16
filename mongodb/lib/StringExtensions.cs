using System;
using System.Collections.Generic;
using System.Text.Json;

namespace paranoid.software.ephemerals.MongoDB
{
    public static class StringExtensions
    {
        public static bool TryToParseJsonString(this string jsonString, out Dictionary<string, List<dynamic>> json)
        {
            json = null;
            var result = true;
            try
            {
                json = JsonSerializer.Deserialize<Dictionary<string, List<dynamic>>>(jsonString);
            }
            catch(Exception)
            {
                result = false;
            }

            return result;
        }
    }
}