namespace Monkey.Object
{

    public class Strings : Object, IHash
    {
        public string Value { get; set; }

        public HashKey HashKey()
        {
            return new HashKey { Value = this.Value.GetHashCode(), Type = this.Type() };
        }

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
