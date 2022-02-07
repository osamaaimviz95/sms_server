using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MetroProcessDispatcher.Internal
{
    public class ParameterCollection
    {
        private List<SqlParameter> _parameters;

        public ParameterCollection()
        {
            _parameters = new List<SqlParameter>();
        }

        public void Clear()
        {
            _parameters.Clear();
        }

        public void Add(string parameterName, object value)
        {
            SqlParameter parameter = new SqlParameter(parameterName, value);
            _parameters.Add(parameter);
        }

        public int Count
        {
            get { return _parameters.Count; }
        }

        public SqlParameter this[int index]
        {
            get
            {
                return _parameters[index];
            }
            set
            {
                _parameters[index] = value;
            }
        }
    }
}
