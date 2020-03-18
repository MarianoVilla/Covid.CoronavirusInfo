using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Covid.Lib
{
    public class StringUtil
    {
        public static bool AnyNullOrWthiteSpace(params string[] TheStrings) => TheStrings.Any(x => string.IsNullOrWhiteSpace(x));
        public static bool AllNullOrWthiteSpace(params string[] TheStrings) => TheStrings.All(x => string.IsNullOrWhiteSpace(x));
    }
}
