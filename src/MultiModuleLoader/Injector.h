#pragma once
#include "pch.h"

class CInjector
{
private:
	static bool FileExists(const wchar_t* wszPath);

public:
	static bool InjectLibrary(DWORD dwProcessId, const wchar_t* wszProcPath, const wchar_t* wszDllPath, const char* szSpoofIP);
};