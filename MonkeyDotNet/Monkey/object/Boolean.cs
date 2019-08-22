namespace Monkey.Object
{
    public class Boolean : Object, IHash
    {
        public bool Value { get; set; }

        public HashKey HashKey()
        {
            long value = Value ? 1 : 0;
            return new HashKey { Type = this.Type(), Value = value };
        }

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