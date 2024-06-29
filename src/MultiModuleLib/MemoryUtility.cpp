#include "pch.h"
#include "MemoryUtility.h"

bool CMemoryUtility::Nop(uintptr_t offset, size_t count)
{
	LPVOID lpOffset = reinterpret_cast<LPVOID>(offset);
	DWORD dwOldProtect = 0;

	if (!VirtualProtect(lpOffset, count, PAGE_READWRITE, &dwOldProtect))
		return false;

	memset(lpOffset, MEMUTIL_ASM_OPCODE_NOP, count);

	return VirtualProtect(lpOffset, count, dwOldProtect, &dwOldProtect);
}

bool CMemoryUtility::SetupHook(E_MEMUTIL_HOOK_TYPE type, uintptr_t src, uintptr_t dest)
{
	DWORD dwOldProtect;
	LPVOID lpSource = reinterpret_cast<LPVOID>(src);
	LPVOID lpDest = reinterpret_cast<LPVOID>(dest);

	if (type == E_MEMUTIL_HOOK_TYPE::LONG_JUMP || type == E_MEMUTIL_HOOK_TYPE::LONG_CALL)
	{
		char instruction[5];
		size_t nInstructionSize = sizeof(instruction);

		instruction[0] = (type == E_MEMUTIL_HOOK_TYPE::LONG_JUMP) ?
			MEMUTIL_ASM_OPCODE_LONG_JUMP : MEMUTIL_ASM_OPCODE_LONG_CALL;

		*(DWORD*)(instruction + 1) = (dest - (src + 5));

		if (!VirtualProtect(lpSource, nInstructionSize, PAGE_EXECUTE_READWRITE, &dwOldProtect))
			return false;

		memcpy(lpSource, instruction, nInstructionSize);

		return VirtualProtect(lpSource, nInstructionSize, dwOldProtect, &dwOldProtect);
	}

	return false;
}

bool CMemoryUtility::Write(uintptr_t offset, const void* data, int length)
{
	LPVOID lpOffset = reinterpret_cast<LPVOID>(offset);
	DWORD dwOldProtect = 0;
	if (!VirtualProtect(lpOffset, length, PAGE_READWRITE, &dwOldProtect))
		return false;

	memcpy(lpOffset, data, length);

	return VirtualProtect(lpOffset, length, dwOldProtect, &dwOldProtect);
}
