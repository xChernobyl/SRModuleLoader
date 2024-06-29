#pragma once
#include "pch.h"

class CProcUtility
{
public:
	//Returns string of zero length if not found.
	static std::wstring GetFileNameByProcessId(const DWORD dwProcessId);

	//Returns 0 on failure.
	static DWORD GetProcessIdByPath(const wchar_t* wszPath);

	//Attempts to create a process in suspended mode. Needs to be resumed once our job is done.
	static bool CreateSuspendedProcess(const wchar_t* wszPath, PROCESS_INFORMATION* pi);
};