using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using log4net;

namespace ImalWebUtilities.model
{

    public class DatabaseService
    {
        public DbProviderFactory MyDataBaseProvider { get; private set; }
        public DbConnection MyDataBaseConn { get; private set; }
        public DbCommand Command { get; private set; }
        private DataSet _ds = new DataSet();
        public string ConnectionString { get; private set; }
        public int DbType { get; private set; }
        public int DbOperationType { get; private set; }
        public string Query { get; set; }
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public int ResponseCode { get; set; }
        public string ResponseDescription { get; set; }


        public DatabaseService(int dbType, string query, int operationType, DatabaseParamaters parameters)
        {
            DbType = dbType;
            DbOperationType = operationType;
            Query = query;
            Parameters = parameters;
        }
        public ILog Logger
        {
            get { return _logger; }
        }

        public void Process()
        {
            PrepareConnection();
            TreatOperation();
        }

        private void PrepareConnection()
        {
            try
            {
                switch (DbType)
                {
                    case 5:
                        ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["imal"].ConnectionString;
                        MyDataBaseProvider = DbProviderFactories.GetFactory("Oracle.ManagedDataAccess");
                        Logger.Info("Connected to Oracle database");

                        break;
                    case 6:
                        ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["local"].ConnectionString;
                        MyDataBaseProvider = DbProviderFactories.GetFactory("System.Data.SqlClient");
                        Logger.Info("Connected to MSSQL database");
                        break;
                }
                MyDataBaseConn = MyDataBaseProvider.CreateConnection();
                if (MyDataBaseConn != null) MyDataBaseConn.ConnectionString = ConnectionString;
                if (MyDataBaseConn != null) MyDataBaseConn.Open();

            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        private void TreatOperation()
        {
            using (Command = MyDataBaseProvider.CreateCommand())
            {
                if (Command != null) Command.CommandText = Query;
                if (Command != null) Command.CommandTimeout = 150;
                if (Command != null) Command.CommandType = CommandType.Text;

                //add the paramaters if any
                //int count = 0;
                foreach (var p in Parameters.Parameters)
                {
                    if (Command != null)
                    {
                        DbParameter param = Command.CreateParameter();
                        param.ParameterName = p.Key;
                        param.Value = p.Value;
                        Command.Parameters.Add(param);
                    }
                    //count++;
                }

                //execute the query 

                switch (DbOperationType)
                {
                    case 1:
                        using (DbDataAdapter reader = MyDataBaseProvider.CreateDataAdapter())
                        {
                            try
                            {
                                //Logger.Info(Query);
                                if (reader != null)
                                {
                                    reader.SelectCommand = Command;
                                    if (reader.SelectCommand != null) reader.SelectCommand.Connection = MyDataBaseConn;
                                    reader.Fill(Ds);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Fatal(ex);
                                ResponseCode = -1; // error treating select query;
                                ResponseDescription = ex.Message;
                            }
                        }
                        break;
                    case 2:
                        try
                        {
                            if (Command != null)
                            {
                                Command.Connection = MyDataBaseConn;
                                ResponseCode = Command.ExecuteNonQuery();
                                ResponseDescription = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Fatal(ex);
                            ResponseCode = -2; // error treating delete query;
                            ResponseDescription = ex.Message;
                        }
                        break;
                    case 3:
                        try
                        {
                            if (Command != null)
                            {
                                Command.Connection = MyDataBaseConn;
                                ResponseCode = Command.ExecuteNonQuery();
                                ResponseDescription = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Fatal(ex);
                            ResponseCode = -3; // error treating update query;
                            ResponseDescription = ex.Message;
                        }
                        break;
                    case 4:
                        try
                        {
                            if (Command != null)
                            {
                                Command.Connection = MyDataBaseConn;
                                if(DbType ==6)
                                {
                                    Command.CommandText += "; select @@IDENTITY ";
                                    ResponseCode = (int)Command.ExecuteScalar();
                                }
                                else
                                {
                                    ResponseCode = Command.ExecuteNonQuery();
                                }
                                
                                
                                ResponseDescription = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Fatal(ex);
                            ResponseCode = -4; // error treating insert query;
                            ResponseDescription = ex.Message;
                        }
                        break;
                }
            }



        }

        public DataSet Ds
        {
            get { return _ds; }
            set { _ds = value; }
        }

        public DatabaseParamaters Parameters { get; set; }
    }
}