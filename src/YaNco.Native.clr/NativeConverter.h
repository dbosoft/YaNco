#pragma once
#include <msclr/marshal_cppstd.h>


using namespace System;
using namespace msclr::interop;


namespace Dbosoft {
	namespace SAP {
		namespace NWRfc {
			namespace Native {

				ref class NativeConverter
				{
				private:
					NativeConverter();

				internal:
					static RfcErrorInfo NativeRfcErrorInfoToManaged(RFC_ERROR_INFO nativeErrorInfo);
					static void ManagedRfcErrorInfoToNative(RfcErrorInfo errorInfo, RFC_ERROR_INFO* nativeErrorInfo);
					static RfcParameterInfo^ NativeRfcParameterInfoToManaged(RFC_PARAMETER_DESC nativeParameterDesc);
					static RfcFieldInfo^ NativeRfcFieldInfoToManaged(RFC_FIELD_DESC nativeFieldDesc);
				};
			}
		}
	}
}