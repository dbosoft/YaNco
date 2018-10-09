
#include "stdafx.h"

#include "HandleWrappers.h"

namespace Contiva {
	namespace SAP {
		namespace NWRfc {


			void ConnectionHandle::DestroyHandle(RFC_CONNECTION_HANDLE handle)
			{
				RfcCloseConnection(handle, NULL);
			}

			void FunctionDescriptionHandle::DestroyHandle(RFC_FUNCTION_DESC_HANDLE handle)
			{
				RfcDestroyFunctionDesc(handle, NULL);
			}

			void TypeDescriptionHandle::DestroyHandle(RFC_TYPE_DESC_HANDLE handle)
			{
				RfcDestroyTypeDesc(handle, NULL);
			}

			void FunctionHandle::DestroyHandle(RFC_FUNCTION_HANDLE handle)
			{
				RfcDestroyFunction(handle, NULL);
			}

			void StructureHandle::DestroyHandle(RFC_STRUCTURE_HANDLE handle)
			{
				RfcDestroyStructure(handle, NULL);
			}

			void TableHandle::DestroyHandle(RFC_TABLE_HANDLE handle)
			{				

				RfcDestroyTable(handle, NULL);
			}
		}
	}
}