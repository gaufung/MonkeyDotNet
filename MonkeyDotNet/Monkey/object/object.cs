using System;

namespace Monkey.Object
{
    public abstract class Object 
    {
        public abstract ObjectType Type();

        public abstract string Inspect(); 
    }
}