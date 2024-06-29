#pragma once
#include "pch.h"

enum class E_MEMUTIL_HOOK_TYPE
{
	LONG_JUMP,
	LONG_CALL,
};

#define MEMUTIL_ASM_OPCODE_LONG_JUMP			0xE9
#define MEMUTIL_ASM_OPCODE_SHORT_JUMP			0xEB
#define MEMUTIL_ASM_OPCODE_LONG_CALL			0xE8
#define MEMUTIL_ASM_OPCODE_NOP					0x90

#define MEMUTIL_WRITE_VALUE(type, offset, value) \
	CMemoryUtility::Write<type>(offset, value)

#define MEMUTIL_WRITE_POINTER(offset, dataPtr, dataLen) \
	CMemoryUtility::Write(offset, dataPtr, dataLen)

#define MEMUTIL_NOP(offset, count) \
	CMemoryUtility::Nop(offset, count)

#define MEMUTIL_READ_BY_PTR_OFFSET(ptr, offset, type) \
	*(type*)(((uintptr_t)ptr) + offset)

#define MEMUTIL_WRITE_BY_PTR_OFFSET(ptr, offset, type, value) \
    *(type*)(((uintptr_t)ptr) + offset) = value;

#define MEMUTIL_SETUP_HOOK(type, src, dest) \
	CMemoryUtility::SetupHook(type, src, dest);

#define MEMUTIL_ADD_PTR(ptr, offset) \
	(((uintptr_t)(ptr)) + offset)

class CMemoryUtility
{
public:
	template<typename T>
	static bool Write(uintptr_t offset, const T& value)
	{
		LPVOID lpOffset = reinterpret_cast<LPVOID>(offset);

		DWORD dwOldProtect = 0;
		if (!VirtualProtect(lpOffset, sizeof(T), PAGE_READWRITE, &dwOldProtect))
			return false;

		*(T*)(offset) = value;

		return VirtualProtect(lpOffset, sizeof(T), dwOldProtect, &dwOldProtect);
	}

	static bool Write(uintptr_t offset, const void* data, int length);
	static bool Nop(uintptr_t offset, size_t count);
	static bool SetupHook(E_MEMUTIL_HOOK_TYPE type, uintptr_t src, uintptr_t dest);
};