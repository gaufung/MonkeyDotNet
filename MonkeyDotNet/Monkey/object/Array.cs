using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Monkey.Object
{
    public class Array : Object
    {
        public IList<Object> Elements { get; set; }
        public override string Inspect()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append(string.Join(",", this.Elements.Select(e =>e.Inspect())));
            sb.Append("]");
            return sb.ToString();
        }

        public override ObjectType Type()
        {
            return ObjectType.ARRAY_OBJ;
        }
    }
}
