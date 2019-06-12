#include "stdafx.h"
#include "NativeConverter.h"

namespace Dbosoft {
	namespace YaNco {
		namespace Native {

				NativeConverter::NativeConverter()
				{
				}

				RfcParameterInfo^ NativeConverter::NativeRfcParameterInfoToManaged(RFC_PARAMETER_DESC nativeParameterDesc)
				{
					return gcnew RfcParameterInfo(
						marshal_as<String^>(nativeParameterDesc.name),
						static_cast<RfcType>(nativeParameterDesc.type),
						static_cast<RfcDirection>(nativeParameterDesc.direction),
						nativeParameterDesc.nucLength,
						nativeParameterDesc.ucLength,
						nativeParameterDesc.decimals,
						marshal_as<String^>(nativeParameterDesc.defaultValue),
						marshal_as<String^>(nativeParameterDesc.parameterText),
						static_cast<bool>(nativeParameterDesc.optional)
					);
				}

				RfcFieldInfo^ NativeConverter::NativeRfcFieldInfoToManaged(RFC_FIELD_DESC nativeFieldDesc)
				{
					return gcnew RfcFieldInfo(
						marshal_as<String^>(nativeFieldDesc.name),
						static_cast<RfcType>(nativeFieldDesc.type),
						nativeFieldDesc.nucLength,
						nativeFieldDesc.ucLength,
						nativeFieldDesc.decimals);
				}

				RfcErrorInfo NativeConverter::NativeRfcErrorInfoToManaged(RFC_ERROR_INFO nativeErrorInfo)
				{
					return RfcErrorInfo(
						(RfcRc)nativeErrorInfo.code,
						(RfcErrorGroup)nativeErrorInfo.group,
						marshal_as<String^>(nativeErrorInfo.key),
						marshal_as<String^>(nativeErrorInfo.message),
						marshal_as<String^>(nativeErrorInfo.abapMsgClass),
						marshal_as<String^>(nativeErrorInfo.abapMsgType),
						marshal_as<String^>(nativeErrorInfo.abapMsgNumber),
						marshal_as<String^>(nativeErrorInfo.abapMsgV1),
						marshal_as<String^>(nativeErrorInfo.abapMsgV2),
						marshal_as<String^>(nativeErrorInfo.abapMsgV3),
						marshal_as<String^>(nativeErrorInfo.abapMsgV4));

				}

				void NativeConverter::ManagedRfcErrorInfoToNative(RfcErrorInfo errorInfo, RFC_ERROR_INFO* nativeErrorInfo)
				{
					nativeErrorInfo->code = static_cast<RFC_RC>(errorInfo.Code);
					nativeErrorInfo->group = static_cast<RFC_ERROR_GROUP>(errorInfo.Group);

					marshal_context^ context = gcnew marshal_context();

					String^ value = errorInfo.Key;
					snprintfU(nativeErrorInfo->key, sizeof(nativeErrorInfo->key - 1), context->marshal_as<const SAP_UC*>(value));
					value = errorInfo.Message;
					snprintfU(nativeErrorInfo->message, sizeof(nativeErrorInfo->message - 1), context->marshal_as<const SAP_UC*>(value));
					value = errorInfo.AbapMsgClass;
					snprintfU(nativeErrorInfo->abapMsgClass, sizeof(nativeErrorInfo->abapMsgClass - 1), context->marshal_as<const SAP_UC*>(value));
					value = errorInfo.AbapMsgType;
					snprintfU(nativeErrorInfo->abapMsgType, sizeof(nativeErrorInfo->abapMsgType - 1), context->marshal_as<const SAP_UC*>(value));
					value = errorInfo.AbapMsgNumber;
					snprintfU(nativeErrorInfo->abapMsgNumber, sizeof(nativeErrorInfo->abapMsgNumber - 1), context->marshal_as<const SAP_UC*>(value));
					value = errorInfo.AbapMsgV1;
					snprintfU(nativeErrorInfo->abapMsgV1, sizeof(nativeErrorInfo->abapMsgV1 - 1), context->marshal_as<const SAP_UC*>(value));
					value = errorInfo.AbapMsgV2;
					snprintfU(nativeErrorInfo->abapMsgV2, sizeof(nativeErrorInfo->abapMsgV2 - 1), context->marshal_as<const SAP_UC*>(value));
					value = errorInfo.AbapMsgV3;
					snprintfU(nativeErrorInfo->abapMsgV3, sizeof(nativeErrorInfo->abapMsgV3 - 1), context->marshal_as<const SAP_UC*>(value));
					value = errorInfo.AbapMsgV4;
					snprintfU(nativeErrorInfo->abapMsgV4, sizeof(nativeErrorInfo->abapMsgV4 - 1), context->marshal_as<const SAP_UC*>(value));

				}
			}
	}
}