using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Monkey.Object
{
    public class HashKey
    {
        public ObjectType Type { get; set; }

        public long Value { get; set; }

        public override bool Equals(object obj)
        {
            HashKey other = obj as HashKey;
            if (other == null)
            {
                return false;
            }
            return other.Type == this.Type && other.Value == this.Value;
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode() ^ this.Value.GetHashCode();
        }
    }


    public class HashPair
    {
        public Object Key { get; set; }

        public Object Value { get; set; }
    }


    public class Hash : Object
    {
        public IDictionary<HashKey, HashPair> Pairs { get; set; }

        public override string Inspect()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append(string.Join(",", Pairs.Select(pair => $"{pair.Value.Key.Inspect()}:{pair.Value.Value.Inspect()}")));
            sb.Append("}");
            return sb.ToString();
        }

        public override ObjectType Type()
        {
            return ObjectType.HASH_OBJ;
        }
    }
}
