using System;

namespace Monkey.Object
{
    public class Boolean : Object
    {
        public bool Value { get; set; }
        public override string Inspect()
        {
            return $"{this.Value}";
        }

        public override ObjectType Type()
        {
            return ObjectType.BOOLEAN_OBJ;
        }
    }
}