using System;
using System.Collections.Generic;
using System.Text;

namespace Monkey.Parser
{
    internal enum Precedence
    {
        LOWEST = 0,
        EQUALS = 1,
        LESSGRATER = 2,
        SUM = 3,
        PRODUCT = 4,
        PREFIX = 5,
        CALL = 6,
        INDEX = 7,
    }
}
