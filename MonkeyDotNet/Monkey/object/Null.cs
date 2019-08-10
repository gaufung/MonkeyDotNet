using System;
namespace Monkey.Object
{
    public class Null : Object
    {
        public override string Inspect()
        {
            return "null";
        }

        public override ObjectType Type()
        {
            return ObjectType.NULL_OBJ;
        }
    }
}
