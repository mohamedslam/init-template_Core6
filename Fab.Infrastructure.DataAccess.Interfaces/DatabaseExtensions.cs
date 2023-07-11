using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;
using System.Data.Common;

namespace Fab.Infrastructure.DataAccess.Interfaces;

public static class DatabaseExtensions
{
    public static Task<List<T>> QuerySqlInterpolatedAsync<T>(this DatabaseFacade databaseFacade,
                                                             FormattableString sql,
                                                             Func<DbDataReader, T> projection,
                                                             CancellationToken cancellationToken) =>
        QuerySqlRawAsync(databaseFacade, sql.Format, sql.GetArguments()!, projection, cancellationToken);

    public static async Task<List<T>> QuerySqlRawAsync<T>(this DatabaseFacade databaseFacade,
                                                          string sql,
                                                          object?[] parameters,
                                                          Func<DbDataReader, T> projection,
                                                          CancellationToken cancellationToken)
    {
        var command = databaseFacade.GetDbConnection()
                                    .CreateCommand();

        var pointer = 0;
        var arguments = new object?[parameters.Length];
        foreach (var parameter in parameters)
        {
            var dbParameter = command.CreateParameter();

            dbParameter.ParameterName = $"p{pointer}";
            dbParameter.Value = parameter;

            arguments[pointer++] = $"@{dbParameter.ParameterName}";
            command.Parameters.Add(dbParameter);
        }

        command.CommandText = string.Format(sql, arguments);

        if (command.Connection!.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync(cancellationToken);
        }

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var results = new List<T>();

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(projection(reader));
        }

        return results;
    }
}