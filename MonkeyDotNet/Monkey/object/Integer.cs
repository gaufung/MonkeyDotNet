namespace Monkey.Object
{
    public class Integer : Object, IHash
    {
        public long Value { get; set; }

        public HashKey HashKey()
        {
            return new HashKey { Type = this.Type(), Value = this.Value };
        }

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