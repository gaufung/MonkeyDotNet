using System;

namespace Monkey.Object
{
    public class Integer : Object
    {
        public long Value { get; set; }
        public override string Inspect()
        {
            return $"{this.Value}";
        }

        public override ObjectType Type()
        {
            return ObjectType.INTEGER_OBJ;
        }
    }
}