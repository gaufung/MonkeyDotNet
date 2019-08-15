using System;
using System.Collections.Generic;
using System.Text;

namespace Monkey.Object
{

    public class Strings : Object
    {
        public string Value { get; set; }


        public override string Inspect()
        {
            return this.Value;
        }

        public override ObjectType Type()
        {
            return ObjectType.STRING_OBJ;
        }
    }
}
