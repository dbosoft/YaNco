using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using WebApi.ApiModel;

namespace WebApi.Shared
{
    [ExcludeFromCodeCoverage]
    public static class CompanyExtensions
    {
        public static EitherAsync<RfcError, IEnumerable<CompanyModel>> GetCompanies(this IRfcContext rfcContext)
        {
            return rfcContext.CallFunction("BAPI_COMPANYCODE_GETLIST",
                Output: f => f
                    .MapTable("COMPANYCODE_LIST", s =>
                        from code in s.GetField<string>("COMP_CODE")
                        from name in s.GetField<string>("COMP_NAME")
                        select new CompanyModel
                        {
                            Code = code,
                            Name = name
                        }));

        }

    }
}
