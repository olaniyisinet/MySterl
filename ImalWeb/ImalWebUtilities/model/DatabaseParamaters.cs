using System.Collections;
using System.Collections.Generic;

namespace ImalWebUtilities.model
{
   
    public class DatabaseParamaters : IEnumerable
    {
        private List<KeyValuePair<string, object>> _parameters  = new List<KeyValuePair<string, object>>();

        public List<KeyValuePair<string, object>> Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }


        public void AddParameter(string name,object value)
        {
            _parameters.Add(new KeyValuePair<string, object>(name,value));
        }


         IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (object o in Parameters)
            {

                if (o == null)
                {
                    break;
                }
                yield return o;
            }
        }
    }
}