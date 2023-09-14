using System.Data;
using System.Runtime.InteropServices.JavaScript;
using Core.Common.HumanTime;
using Dapper;

namespace Core.Database;

public class HumanTimeTypeHandler : SqlMapper.TypeHandler<HumanTime>
{
    private const byte DataSizeBits = 40;
    private const ulong DataMask = 0xFFFFFFFFFF;
    private const ulong TypeMask = ~DataMask;
    
    public override void SetValue(IDbDataParameter parameter, HumanTime value)
    {
        ulong serialized = value switch
        {
            HumanDate date => (ulong)((1L << DataSizeBits) | (uint)date.Date.DayNumber),
            HumanDateTime dateTime => (2L << DataSizeBits) | (ulong)dateTime.DateTime.ToUnixTimeSeconds(),
            HumanTimeWindow window => (3L << DataSizeBits) | ((ulong)window.WindowType << 32) | (uint)window.StartDate.DayNumber,
            HumanTimeUnknown => 0,
            _ => 0
        };

        parameter.Value = serialized;
    }

    public override HumanTime Parse(object value)
    {
        var serialized = (ulong)value;

        var data = serialized & DataMask;
        var timeType = (serialized & TypeMask) >> DataSizeBits;
        return timeType switch
        {
            0 => HumanTime.Unknown,
            1 => HumanTime.Date(DateOnly.FromDayNumber((int)data)),
            2 => HumanTime.DateTime(DateTimeOffset.FromUnixTimeSeconds((long)data)),
            3 => new HumanTimeWindow
            {
                WindowType = (HumanTimeWindowType)((data & ~0xFFFFFFFF) >> 32),
                StartDate = DateOnly.FromDayNumber((int)(data & 0xFFFFFFFF))
            },
            _ => HumanTime.Unknown,
        };
    }
}