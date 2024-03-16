using System.Collections.Generic;
using System.Linq;
using Dbosoft.YaNco.TypeMapping;
using LanguageExt;

namespace Dbosoft.YaNco;

internal class Structure : TypeDescriptionDataContainer, IStructure
{
    private readonly IStructureHandle _handle;

    public Structure(IStructureHandle handle, SAPRfcDataIO io) : base(handle, io)
    {
        _handle = handle;
    }


    public Either<RfcError, RfcFieldInfo[]> GetFieldInfos()
    {
        return IO.GetTypeDescription(Handle).Use(used => used
            .Bind( handle =>
            {
                return IO.GetTypeFieldCount(handle).Bind(fieldCount =>
                {
                    return Enumerable.Range(0, fieldCount).Map(i =>
                        IO.GetTypeFieldDescription(handle, i)).Traverse(l=>l);
                });

            })).Map(r => r.ToArray());
    }

    public Either<RfcError, IDictionary<string, AbapValue>> ToDictionary()
    {
        return GetFieldInfos()
            .Bind(fields =>
                fields.Map(fieldInfo =>
                    from fieldValue in IO.GetFieldValue<AbapValue>(Handle, () => fieldInfo)
                    from fieldName in Prelude.Right(fieldInfo.Name).Bind<RfcError>()
                    select (fieldName, fieldValue)
                ).Traverse(l => l))
            .Map(l => l.ToDictionary(
                v => v.fieldName,
                v => v.fieldValue))
            .Map(d => (IDictionary<string,AbapValue>) d);

    }

    public Either<RfcError, Unit> SetFromDictionary<T>(IDictionary<string, T> dictionary)
    {
        return GetFieldInfos()
            .Bind(fields =>
                fields.Map(fieldInfo => 
                        !dictionary.ContainsKey(fieldInfo.Name) 
                            ? Unit.Default
                            : IO.SetFieldValue(Handle, dictionary[fieldInfo.Name], () => fieldInfo)
                    )
                    .Traverse(l => l))
            .Map(_ => Unit.Default);
    }

    public Either<RfcError, Unit> SetFromString(string content)
    {
        return IO.SetStructure(_handle, content);
    }
}