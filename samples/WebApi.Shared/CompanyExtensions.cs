using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using WebApi.ApiModel;

namespace WebApi.Shared
{
    public static class CompanyExtensions
    {
        public static EitherAsync<RfcErrorInfo, IEnumerable<CompanyModel>> GetCompanies(this IRfcContext rfcContext)
        {
            return rfcContext.CallFunction("BAPI_COMPANYCODE_GETLIST",
                Output: f => f
                    .MapTable("COMPANYCODE_LIST", s =>
                        from code in s.GetField<string>("COMP_CODE")
                        from name in s.GetField<string>("COMP_NAME")
                        select new CompanyModel
                        {
                            CompanyCode = code,
                            CompanyName = name
                        }));

        }

    }
}
