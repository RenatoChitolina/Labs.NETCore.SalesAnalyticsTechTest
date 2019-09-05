using SalesAnalytics.Infrastructure.Enums;
using System;

namespace SalesAnalytics.Infrastructure.Helpers
{
    public static class LineTypeHelper
    {
        public static LineType StringToLineType(string value, LineType defaultValue = default(LineType))
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return Enum.TryParse(value, true, out LineType result) 
                ? result 
                : defaultValue;
        }
    }
}
