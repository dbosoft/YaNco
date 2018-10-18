
#include "stdafx.h"

#include "HandleWrappers.h"

namespace Contiva {
	namespace SAP {
		namespace NWRfc {

			namespace Native {

				void ConnectionHandle::DestroyHandle(RFC_CONNECTION_HANDLE handle)
				{
					RfcCloseConnection(handle, NULL);
				}

				void FunctionDescriptionHandle::DestroyHandle(RFC_FUNCTION_DESC_HANDLE handle)
				{
					RFC_RC rc = RfcDestroyFunctionDesc(handle, NULL);
				}

				void TypeDescriptionHandle::DestroyHandle(RFC_TYPE_DESC_HANDLE handle)
				{
					RFC_RC rc = RfcDestroyTypeDesc(handle, NULL);
				}

				void FunctionHandle::DestroyHandle(RFC_FUNCTION_HANDLE handle)
				{
					RFC_RC rc = RfcDestroyFunction(handle, NULL);
				}

				void StructureHandle::DestroyHandle(RFC_STRUCTURE_HANDLE handle)
				{
					RFC_RC rc = RfcDestroyStructure(handle, NULL);
				}

				void TableHandle::DestroyHandle(RFC_TABLE_HANDLE handle)
				{

					RFC_RC rc = RfcDestroyTable(handle, NULL);
				}
			}
		}
	}
}