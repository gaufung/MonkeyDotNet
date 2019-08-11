using System;

namespace Monkey.Object
{
    public class Error : Object
    {
        public string Message { get; set; }

        public override ObjectType Type()
        {
            return ObjectType.ERROR_OBJ;
        }

        public override string Inspect() 
        {
            return $"ERROR: {this.Message}";
        }
    }
}