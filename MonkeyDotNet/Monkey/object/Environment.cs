using System.Collections.Generic;

namespace Monkey.Object
{
    public class Environment
    {
        public IDictionary<string, Object> Store { get; private set; }

        private Environment _outer;

        public Environment()
        {
            Store = new Dictionary<string, Object>();
            _outer = null;
        }

        public Environment(Environment outer)
        {
             Store = new Dictionary<string, Object>();
            _outer = outer;
        }

        public bool Exist(string name)
        {
            if(this.Store.ContainsKey(name))
            {
                return true;
            }
            else 
            {
                if(this._outer!=null)
                {
                    return this._outer.Exist(name);
                }
                else
                {
                    return false;
                }  
            }
        }

        public Object Get(string name)
        {
            return this.Store[name];
        }

        public void Set(string name, Object obj) 
        {
            this.Store[name] = obj;
        }
    }
}