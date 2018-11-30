#pragma once
#include <sstream> //for std::stringstream 
#include <string>  //for std::string


using namespace System;

namespace Contiva {
	namespace SAP {
		namespace NWRfc {

			namespace Native {

				public interface class IDataContainerHandle: Contiva::SAP::NWRfc::IDataContainerHandle
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

				public:
					virtual String^ ToString() override
					{
						const void * address = static_cast<const void*>(Handle);
						std::stringstream ss;
						ss << address;
						std::string name = ss.str();

						return gcnew String(name.c_str());
					}
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



				public ref class ConnectionHandle : HandleBase<RFC_CONNECTION_HANDLE>, Contiva::SAP::NWRfc::IConnectionHandle
				{
				internal:
					ConnectionHandle(RFC_CONNECTION_HANDLE handle) : HandleBase(handle, true) { }

				protected:
					virtual void DestroyHandle(RFC_CONNECTION_HANDLE handle) override;
				};

				public ref class FunctionDescriptionHandle : HandleBase<RFC_FUNCTION_DESC_HANDLE>, Contiva::SAP::NWRfc::IFunctionDescriptionHandle
				{
				internal:
					FunctionDescriptionHandle(RFC_FUNCTION_DESC_HANDLE handle, bool selfDestroy) : HandleBase(handle, selfDestroy) { }

				protected:
					virtual void DestroyHandle(RFC_FUNCTION_DESC_HANDLE handle) override;
				};

				public ref class TypeDescriptionHandle : HandleBase<RFC_TYPE_DESC_HANDLE>, Contiva::SAP::NWRfc::ITypeDescriptionHandle
				{
				internal:
					TypeDescriptionHandle(RFC_TYPE_DESC_HANDLE handle, bool selfDestroy) : HandleBase(handle, selfDestroy) { }

				protected:
					virtual void DestroyHandle(RFC_TYPE_DESC_HANDLE handle) override;
				};

				public ref class FunctionHandle : DataContainerBase<RFC_FUNCTION_HANDLE>, Contiva::SAP::NWRfc::IFunctionHandle
				{
				internal:
					FunctionHandle(RFC_FUNCTION_HANDLE handle) : DataContainerBase(handle, true) { }

				protected:
					virtual void DestroyHandle(RFC_FUNCTION_HANDLE handle) override;
				};

				public ref class StructureHandle : DataContainerBase<RFC_STRUCTURE_HANDLE>, Contiva::SAP::NWRfc::IStructureHandle
				{
				internal:
					StructureHandle(RFC_STRUCTURE_HANDLE handle, bool selfDestroy) : DataContainerBase(handle, selfDestroy) { }

				protected:
					virtual void DestroyHandle(RFC_STRUCTURE_HANDLE handle) override;
				};

				public ref class TableHandle : DataContainerBase<RFC_TABLE_HANDLE>, Contiva::SAP::NWRfc::ITableHandle
				{
				internal:
					TableHandle(RFC_TABLE_HANDLE handle, bool selfDestroy) : DataContainerBase(handle, selfDestroy) { }

				protected:
					virtual void DestroyHandle(RFC_TABLE_HANDLE handle) override;
				};
			}
		}
	}
}