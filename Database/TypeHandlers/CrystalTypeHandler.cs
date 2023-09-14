using System.Data;
using Dapper;

namespace Database.TypeHandlers;

public class CrystalTypeHandler : SqlMapper.TypeHandler<Crystal>
{
    public override void SetValue(IDbDataParameter parameter, Crystal crystal)
    {
        parameter.Value = crystal.Value;
    }

    public override Crystal Parse(object value)
    {
        return new Crystal(Convert.ToUInt64(value));
    }
}