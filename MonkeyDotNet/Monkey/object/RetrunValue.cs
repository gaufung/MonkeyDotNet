using System;

namespace Monkey.Object
{
    public class ReturnValue : Object
    {
        public Object Value { get; set; }

        public override ObjectType Type()
        {
            return ObjectType.RETURN_VALUE_OBJ;
        }

        public override string Inspect() 
        {
            return this.Value.Inspect();
        }
    }
}