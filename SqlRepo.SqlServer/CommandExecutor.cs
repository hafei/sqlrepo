using System;
using System.Data;
using System.Data.SqlClient;

namespace SqlRepo.SqlServer
{
    public class CommandExecutor : ICommandExecutor
    {
        private const int CommandTimeout = 300000;
        private readonly string connectionString = string.Empty;
        private readonly ISqlLogger logger;

        public CommandExecutor(ISqlLogger logger)
        {
            this.logger = logger;
        }

        public int ExecuteNonQuery(string sql)
        {
            this.LogQuery(sql);
            using(var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandTimeout = CommandTimeout;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    return command.ExecuteNonQuery();
                }
            }
        }

        public IDataReader ExecuteReader(string sql)
        {
            this.LogQuery(sql);
            var connection = new SqlConnection(this.connectionString);
            connection.Open();
            using(var command = connection.CreateCommand())
            {
                command.CommandTimeout = CommandTimeout;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public IDataReader ExecuteStoredProcedure(string name,
            params ParameterDefinition[] parametersDefinitions)
        {
            this.logger.Log($"Executing Procedure: {name}");
            var connection = new SqlConnection(this.connectionString);
            connection.Open();
            using(var command = connection.CreateCommand())
            {
                command.CommandTimeout = CommandTimeout;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = name;
                foreach(var parametersDefinition in parametersDefinitions)
                {
                    command.Parameters.AddWithValue(parametersDefinition.Name, parametersDefinition.Value);
                }
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        private void LogQuery(string sql)
        {
            this.logger.Log($"Executing SQL:{Environment.NewLine}{sql}");
        }
    }
}