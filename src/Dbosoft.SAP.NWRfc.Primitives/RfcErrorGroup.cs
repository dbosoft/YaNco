using System;
using System.Runtime.InteropServices;
using System.Security;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Contiva.SAP.NWRfc
{
    /// <summary>
    /// <para>Groups several error conditions together, depending on the &quot;layer&quot; to which they belong.</para>
    /// <para>Used in the structure #RFC_ERROR_INFO::group.</para>
    /// </summary>
    public enum RfcErrorGroup
    {
        /// <summary>OK</summary>
        OK = 0,
        /// <summary>ABAP Exception raised in ABAP function modules</summary>
        ABAP_APPLICATION_FAILURE = 1,
        /// <summary>ABAP Message raised in ABAP function modules or in ABAP runtime of the backend (e.g Kernel)</summary>
        ABAP_RUNTIME_FAILURE = 2,
        /// <summary>Error message raised when logon fails</summary>
        LOGON_FAILURE = 3,
        /// <summary>Problems with the network connection (or backend broke down and killed the connection)</summary>
        COMMUNICATION_FAILURE = 4,
        /// <summary>Problems in the RFC runtime of the external program (i.e &quot;this&quot; library)</summary>
        EXTERNAL_RUNTIME_FAILURE = 5,
        /// <summary>Problems in the external program (e.g in the external server implementation)</summary>
        EXTERNAL_APPLICATION_FAILURE = 6,
        /// <summary>Problems raised in the authorization check handler provided by the external server implementation</summary>
        EXTERNAL_AUTHORIZATION_FAILURE = 7
    }

}
