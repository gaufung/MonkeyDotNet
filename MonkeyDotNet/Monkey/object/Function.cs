using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Monkey.Object
{
    public class Function:Object
    {
        public IList<Ast.Identifier> Parameters { get; set; }

        public Ast.BlockStatement Body { get; set; }

        public Environment Env { get; set; }

        public override string Inspect()
        {
            var sb = new StringBuilder();
            sb.Append("fn");
            sb.Append("(");
            sb.Append(string.Join(",", this.Parameters.Select(p=>p.ToString())));
            sb.Append("){"+System.Environment.NewLine);
            sb.Append(this.Body.ToString());
            sb.Append(System.Environment.NewLine+"}");
            return sb.ToString();
        }

        public override ObjectType Type()
        {
            return ObjectType.FUNCTION_OBJ;
        }
    }
}