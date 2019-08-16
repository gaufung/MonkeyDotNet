using System;
using System.Collections.Generic;
using System.Text;

namespace Monkey.Object
{
    public class Builtin : Object
    {

        public Func<Object[], Object> Fn { get; set; }

        public override string Inspect()
        {
            return "bulitin function";
        }

        public override ObjectType Type()
        {
            return ObjectType.BUILTIN_OBJ;
        }
    }
}
