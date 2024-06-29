#include "pch.h"
#include "Injector.h"
#include "ProcUtility.h"

extern "C" __declspec(dllimport) int EntryPoint(const char* szSpoofIP);

bool CInjector::FileExists(const wchar_t* wszPath)
{
	std::ifstream fs(wszPath);

	if (!fs)
		return false;

	fs.close();
	return true;
}

bool CInjector::InjectLibrary(DWORD  dwProcessId, const wchar_t* wszProcPath, const wchar_t* wszDllPath, const char* szSpoofIP)
{
	if (!CInjector::FileExists(wszProcPath))
		return false;

	if (!CInjector::FileExists(wszDllPath))
		return false;

	if (dwProcessId == 0)
		return false;

	HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, dwProcessId);

	if (hProcess == INVALID_HANDLE_VALUE || hProcess == NULL)
		return false;

	size_t nDllPathLen = lstrlenW(wszDllPath) * 2;
	size_t nSpoofIpLen = strlen(szSpoofIP);

	void* dllPathMem = VirtualAllocEx(hProcess, NULL, nDllPathLen, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
	void* spoofIpMem = VirtualAllocEx(hProcess, NULL, nSpoofIpLen, MEM_COMMIT, PAGE_EXECUTE_READWRITE);

	if (dllPathMem == nullptr)
	{
		CloseHandle(hProcess);
		return false;
	}

	if (spoofIpMem == nullptr)
	{
		CloseHandle(hProcess);
		return false;
	}

	BOOL isDllPathWritten = WriteProcessMemory(hProcess, dllPathMem, wszDllPath, nDllPathLen, NULL);
	BOOL isSpoofIpWritten = WriteProcessMemory(hProcess, spoofIpMem, szSpoofIP, nSpoofIpLen, NULL);

	if (!isDllPathWritten)
	{
		CloseHandle(hProcess);
		return false;
	}

	if (!isSpoofIpWritten)
	{
		CloseHandle(hProcess);
		return false;
	}

	HMODULE hTargetLib = LoadLibraryW(wszDllPath);

	auto lpLoadLibrary = (LPTHREAD_START_ROUTINE)(GetProcAddress(LoadLibraryW(L"Kernel32.dll"), "LoadLibraryW"));
	auto lpEntryPoint = (LPTHREAD_START_ROUTINE)(GetProcAddress(hTargetLib, "EntryPoint"));

	if (lpLoadLibrary == nullptr)
	{
		CloseHandle(hProcess);
		return false;
	}

	if (lpEntryPoint == nullptr)
	{
		CloseHandle(hProcess);
		return false;
	}

	

	HANDLE hRemoteThread = CreateRemoteThread(hProcess, NULL, 0, lpLoadLibrary, dllPathMem, 0, NULL);
	if (hRemoteThread == INVALID_HANDLE_VALUE || hRemoteThread == NULL)
	{
		CloseHandle(hProcess);
		return false;
	}

	WaitForSingleObject(hRemoteThread, INFINITE);

	hRemoteThread = CreateRemoteThread(hProcess, NULL, 0, lpEntryPoint, spoofIpMem, 0, NULL);
	if (hRemoteThread == INVALID_HANDLE_VALUE || hRemoteThread == NULL)
	{
		CloseHandle(hProcess);
		return false;
	}

	//maybe, actually no need to wait for this one..
	WaitForSingleObject(hRemoteThread, INFINITE);
	//CloseHandle(hRemoteThread);

	return true;
}