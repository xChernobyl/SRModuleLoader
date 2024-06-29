#include "pch.h"
#include "ProcUtility.h"
#include <Psapi.h>
#include <TlHelp32.h>

//Returns string of zero length if not found.
std::wstring CProcUtility::GetFileNameByProcessId(const DWORD dwProcessId)
{
	HANDLE hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, dwProcessId);
	if (hProcess == INVALID_HANDLE_VALUE || hProcess == NULL)
		return std::wstring();

	wchar_t tmpPath[MAX_PATH] = { 0 };

	if (GetModuleFileNameEx(hProcess, NULL, tmpPath, MAX_PATH) == FALSE)
		return std::wstring();

	CloseHandle(hProcess);

	return std::wstring(tmpPath);
}

//Returns 0 on failure.
DWORD CProcUtility::GetProcessIdByPath(const wchar_t* wszPath)
{
	HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);

	PROCESSENTRY32 snapshot = { 0 };
	snapshot.dwSize = sizeof(decltype(snapshot));

	if (hSnapshot == INVALID_HANDLE_VALUE)
		return 0;

	if (Process32First(hSnapshot, &snapshot) == FALSE)
		return false;

	//Trasform path to lowercase.
	std::wstring pathCopy(wszPath);
	std::transform(pathCopy.begin(), pathCopy.end(), pathCopy.begin(), std::tolower);

	DWORD dwProcessId = 0;

	while (Process32Next(hSnapshot, &snapshot))
	{
		std::wstring fullProcPath = CProcUtility::GetFileNameByProcessId(snapshot.th32ProcessID);

		//Could not obtain the process path.
		if (fullProcPath.length() == 0)
			continue;

		//Transform current process path to lowercase.
		std::transform(fullProcPath.begin(), fullProcPath.end(), fullProcPath.begin(), std::tolower);

		if (wcscmp(fullProcPath.c_str(), pathCopy.c_str()) == 0)
		{
			dwProcessId = snapshot.th32ProcessID;
			printf("%s -> The target process ID is %d\n",
				__FUNCTION__, dwProcessId);
			break;
		}
	}

	CloseHandle(hSnapshot);

	if (dwProcessId == 0)
	{
		printf("%s -> Failed to obtain processs ID.\n",
			__FUNCTION__);
	}
	return dwProcessId;
}

//Attempts to create a process in suspended mode. Needs to be resumed once our job is done.
bool CProcUtility::CreateSuspendedProcess(const wchar_t* wszPath, PROCESS_INFORMATION* pi)
{
	STARTUPINFO startInfo = { 0 };
	startInfo.cb = sizeof(decltype(startInfo));

	if (CreateProcess(wszPath, NULL, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NEW_CONSOLE, NULL, NULL, &startInfo, pi) == FALSE)
	{

		wprintf(L"%s -> Failed to create process, path (%s)\n",
			__FUNCTIONW__, wszPath);
		return false;
	}

	return true;
}