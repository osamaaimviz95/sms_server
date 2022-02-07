using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Threading;

using System.Data.SqlClient;

using Common;

namespace MetroProcessDispatcher.Internal
{
    public class Database : IDisposable
    {
        #region Fields...
        private string _dbconn;
        private SqlConnection _con;
        private SqlTransaction _trn;
        private SqlCommand _cmd;

        private object sync;
        #endregion

        #region Constructors...
        public Database(string dbconn)
        {
            sync = new object();

            _dbconn = dbconn;
            _con = new SqlConnection();
            _trn = null;
            _cmd = null;
        }
        #endregion

        #region Exposed Methods...
        private void SetCommand(string sql, ParameterCollection prms)
        {
            lock (sync)
            {
                if (_cmd != null)
                    _cmd.CommandText = sql;
                else
                {
                    if (_trn != null)
                        _cmd = new SqlCommand(sql, _con, _trn);
                    else
                        _cmd = new SqlCommand(sql, _con);
                }

                _cmd.Parameters.Clear();
               
                if (prms != null)
                    for (int i = 0; i <= prms.Count  - 1; i++)
                        _cmd.Parameters.Add(prms[i]);
            }
        }

        public int ExecuteNonQuery(string sql, ParameterCollection prms)
        {
            SetCommand(sql, prms);

            lock (sync)
            {
                return _cmd.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string sql, ParameterCollection prms)
        {
            SetCommand(sql, prms);

            lock (sync)
            {
                return _cmd.ExecuteScalar();
            }
        }

        public SqlDataReader ExecuteReader(string sql, ParameterCollection prms)
        {
            SetCommand(sql, prms);

            lock (sync)
            {
                return _cmd.ExecuteReader();
            }
        }

        public void Open()
        {
            lock (sync)
            {
                _con.ConnectionString = _dbconn;
                _con.Open();
            }
        }

        public void BeginTransaction()
        {
            lock (sync)
            {
                _con.ConnectionString = _dbconn;
            
                _con.Open();
                _trn = _con.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            lock (sync)
            {
                if (_trn != null)
                    _trn.Commit();
            }
        }

        public void RollbackTransaction()
        {
            lock (sync)
            {
                if (_trn != null)
                    _trn.Rollback();
            }
        }

        public void Dispose()
        {
            lock (sync)
            {
                if (_cmd != null)
                    _cmd.Dispose();

                _con.Close();
                _con.Dispose();
            }
        }
        #endregion

        #region Helper Methods...
        public int GetInt(object value)
        {
            return value == DBNull.Value ? NullType.Int : Convert.ToInt32(value);
        }

        public object SetInt(int value)
        {
            if (value == NullType.Int)
                return DBNull.Value;

            return value;
        }

        public long GetLong(object value)
        {
            return value == DBNull.Value ? NullType.Long : Convert.ToInt64(value);
        }

        public object SetLong(long value)
        {
            if (value == NullType.Long)
                return DBNull.Value;

            return value;
        }

        public double GetDouble(object value)
        {
            return value == DBNull.Value ? NullType.Double : Convert.ToDouble(value);
        }

        public object SetDouble(double value)
        {
            if (value == NullType.Double)
                return DBNull.Value;

            return value;
        }

        public decimal GetDecimal(object value)
        {
            return value == DBNull.Value ? NullType.Decimal : Convert.ToDecimal(value);
        }

        public object SetDecimal(decimal value)
        {
            if (value == NullType.Decimal)
                return DBNull.Value;

            return value;
        }

        public string GetString(object value)
        {
            return value == DBNull.Value ? NullType.String : Convert.ToString(value);
        }

        public object SetString(string value)
        {
            if (value == NullType.String)
                return DBNull.Value;

            return value;
        }

        public DateTime GetDateTime(object value)
        {
            return value == DBNull.Value ? NullType.DateTime : Convert.ToDateTime(value);
        }

        public object SetDateTime(DateTime value)
        {
            if (value == NullType.DateTime)
                return DBNull.Value;

            return value;
        }
        #endregion
    }
}
