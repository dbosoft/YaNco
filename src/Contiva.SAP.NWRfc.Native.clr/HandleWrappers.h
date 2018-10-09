#pragma once

using namespace System;

namespace Contiva {
	namespace SAP {
		namespace NWRfc {


			public interface class IDataContainerHandle
			{
				property DATA_CONTAINER_HANDLE DCHandle {
					DATA_CONTAINER_HANDLE get();
				}
			};


			template<typename T>
			public ref class HandleBase abstract
			{
			private:
				T _handle;
				bool _selfDestroy;

			protected:
				HandleBase(T handle, bool selfDestroy)
				{
					this->_handle = handle;
					this->_selfDestroy = selfDestroy;

				}
			internal:

				property T Handle {
					T get() {
						return _handle;
					}
				}

				~HandleBase()
				{
					if (this->_handle != NULL && _selfDestroy)
						DestroyHandle(this->_handle);

					this->_handle = NULL;
				}

			protected:
				virtual void DestroyHandle(T handle) abstract;
			};

			template<typename T>
			public ref class DataContainerBase abstract : HandleBase<T>, IDataContainerHandle
			{
			internal:
				virtual property DATA_CONTAINER_HANDLE DCHandle {
					virtual DATA_CONTAINER_HANDLE get() = IDataContainerHandle::DCHandle::get{
						return Handle;
					}
				}
			protected:
				DataContainerBase(T handle, bool selfDestroy) : HandleBase(handle, selfDestroy) {}
			};



			public ref class ConnectionHandle : HandleBase<RFC_CONNECTION_HANDLE>
			{
			internal:
				ConnectionHandle(RFC_CONNECTION_HANDLE handle) : HandleBase(handle, true) { }

			protected:
				virtual void DestroyHandle(RFC_CONNECTION_HANDLE handle) override;
			};

			public ref class FunctionDescriptionHandle : HandleBase<RFC_FUNCTION_DESC_HANDLE>
			{
			internal:
				FunctionDescriptionHandle(RFC_FUNCTION_DESC_HANDLE handle, bool selfDestroy) : HandleBase(handle, selfDestroy) { }

			protected:
				virtual void DestroyHandle(RFC_FUNCTION_DESC_HANDLE handle) override;
			};

			public ref class TypeDescriptionHandle : HandleBase<RFC_TYPE_DESC_HANDLE>
			{
			internal:
				TypeDescriptionHandle(RFC_TYPE_DESC_HANDLE handle, bool selfDestroy) : HandleBase(handle, selfDestroy) { }

			protected:
				virtual void DestroyHandle(RFC_TYPE_DESC_HANDLE handle) override;
			};

			public ref class FunctionHandle : DataContainerBase<RFC_FUNCTION_HANDLE>
			{
			internal:
				FunctionHandle(RFC_FUNCTION_HANDLE handle) : DataContainerBase(handle, true) { }

			protected:
				virtual void DestroyHandle(RFC_FUNCTION_HANDLE handle) override;
			};

			public ref class StructureHandle : DataContainerBase<RFC_STRUCTURE_HANDLE>
			{
			internal:
				StructureHandle(RFC_STRUCTURE_HANDLE handle, bool selfDestroy) : DataContainerBase(handle, selfDestroy) { }

			protected:
				virtual void DestroyHandle(RFC_STRUCTURE_HANDLE handle) override;
			};

			public ref class TableHandle : DataContainerBase<RFC_TABLE_HANDLE>
			{
			internal:
				TableHandle(RFC_TABLE_HANDLE handle, bool selfDestroy) : DataContainerBase(handle, selfDestroy) { }

			protected:
				virtual void DestroyHandle(RFC_TABLE_HANDLE handle) override;
			};
		}
	}
}