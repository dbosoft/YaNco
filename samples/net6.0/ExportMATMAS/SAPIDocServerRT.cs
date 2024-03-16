using Dbosoft.YaNco;
using LanguageExt;
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace ExportMATMAS;

public static class SAPIDocServer<RT> where RT : 
    struct, HasSAPRfcServer<RT>, HasSAPRfcFunctions<RT>, HasSAPRfcConnection<RT>, HasSAPRfcLogger<RT>, HasSAPRfcData<RT>
{
    public static Aff<RT, Unit> processInboundIDoc(CalledFunction<RT> cf, TransactionManager<MaterialMasterRecord> transactionManager) => cf
        .Input(i =>

            // extract IDoc data from incoming RFC call
            from data in i.MapTable("IDOC_DATA_REC_40",
                s =>
                    from iDocNo in s.GetField<string>("DOCNUM")
                    from segment in s.GetField<string>("SEGNAM")
                    from segmentNo in s.GetField<int>("SEGNUM")
                    from parentNo in s.GetField<int>("PSGNUM")
                    from level in s.GetField<int>("HLEVEL")
                    from data in s.GetField<string>("SDATA")
                    select new IDocDataRecord(iDocNo, segment, segmentNo, parentNo, level, data)
            )
            select data.ToSeq())
        .Process(data =>

            from state in SAPRfcServer<RT>.getServerAttributes(cf.RfcHandle)
            from guardIsTa in Prelude.guardnot(
                state.CallType != RfcCallType.QUEUED && state.CallType != RfcCallType.TRANSACTIONAL,
                RfcError.Error($"Invalid call type {state.CallType}", RfcRc.RFC_EXTERNAL_FAILURE).AsError)
            from guardTaEmpty in Prelude.guardnot(string.IsNullOrWhiteSpace(state.TransactionId),
                RfcError.Error("Missing transaction id", RfcRc.RFC_EXTERNAL_FAILURE).AsError)

            // open a IRfcContext to call back to sender
            from clientCall in cf.UseRfcContext(context =>

                // get current transaction
                from ta in transactionManager.GetTransaction(state.TransactionId)
                    .ToEither(RfcError.Error(RfcRc.RFC_EXTERNAL_FAILURE))
                    .ToEff(l => l)
                // open a client connection to sender for metadata lookup
                from connection in context.GetConnection()
                from materialMaster in ExtractMaterialMaster(connection, data).ToAff(l => l)

                from unit in SetTransactionData(ta, materialMaster)
                select unit)
            select clientCall
        )
        .NoReply();


    private static Eff<Unit> SetTransactionData(
        TransactionStateRecord<MaterialMasterRecord> ta, MaterialMasterRecord materialMaster)
    {
        ta.Data = materialMaster;
        ta.State = TransactionState.Executed;
        return Prelude.unitEff;
    }


    private static EitherAsync<RfcError, MaterialMasterRecord> ExtractMaterialMaster(IConnection connection,
        Seq<IDocDataRecord> data)
    {
        return

            //extract some client data of material master
            from clientSegment in FindRequiredSegment("E1MARAM", data)
            from material in MapSegment(connection, clientSegment, s =>
                from materialNo in s.GetField<string>("MATNR")
                from unit in s.GetField<string>("MEINS")
                select new
                {
                    MaterialNo = materialNo,
                    ClientData = new ClientData(unit)
                })

            //extract descriptions data of material master
            from descriptionData in MapSegments(connection, FindSegments("E1MAKTM", data), s =>
                from language in s.GetField<string>("SPRAS_ISO")
                from description in s.GetField<string>("MAKTX")
                select new DescriptionData(language, description))

            //extract some plant data of material master
            from plantData in MapSegments(connection, FindSegments("E1MARCM", data), s =>
                from plant in s.GetField<string>("WERKS")
                from purchasingGroup in s.GetField<string>("EKGRP")
                select new PlantData(plant, purchasingGroup)
            )
            select new MaterialMasterRecord(
                material.MaterialNo,
                material.ClientData,
                descriptionData.ToArray(),
                plantData.ToArray());
    }

    private static EitherAsync<RfcError, T> MapSegment<T>(IConnection connection,
        IDocDataRecord data, Func<IStructure, Either<RfcError, T>> mapFunc)
    {
        //to convert the segment we create a temporary structure of the segment definition type
        //and "move" the segment data into it. 
        return connection.CreateStructure(MatmasTypes.Segment2Type[data.Segment]).Use(structure =>
        {
            return from _ in structure.Bind(s => s.SetFromString(data.Data).ToAsync())
                from res in structure.Bind(s => mapFunc(s).ToAsync())
                select res;
        });

    }

    private static EitherAsync<RfcError, Seq<T>> MapSegments<T>(IConnection connection,
        Seq<IDocDataRecord> data, Func<IStructure, Either<RfcError, T>> mapFunc)
    {
        return data.Map(segment => MapSegment(connection, segment, mapFunc))
            .TraverseSerial(l => l);
    }

    private static EitherAsync<RfcError, IDocDataRecord> FindRequiredSegment(
        string typeName, Seq<IDocDataRecord> records)
    {
        var segmentName = MatmasTypes.Type2Segment[typeName];
        return records.Find(x => x.Segment == segmentName)
            .ToEither(RfcError.Error($"Segment {segmentName} not found"))
            .ToAsync();
    }

    private static Seq<IDocDataRecord> FindSegments(
        string typeName, Seq<IDocDataRecord> records)
    {
        var segmentName = MatmasTypes.Type2Segment[typeName];
        return records.Filter(x => x.Segment == segmentName);
    }

    // for a known IDoc type you used fixed segment to type mapping
    // a more generic way would be looking up segment names from RFM IDOCTYPE_READ_COMPLETE
}