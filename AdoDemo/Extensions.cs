using System.Data;
using System.Data.Common;

public static class DbExtensions
{
    public static DbParameter AddWithValue(
        this DbParameterCollection parameters,
        string parameterName,
        object? value,
        DbType dbType,
        DbProviderFactory factory,
        ParameterDirection direction = ParameterDirection.Input
    )
    {
        var parameter = factory.CreateParameter();
        parameter.DbType = dbType;
        parameter.ParameterName = parameterName;
        parameter.Value = value;
        parameter.Direction = direction;

        parameters.Add(parameter);

        return parameter;
    }
}