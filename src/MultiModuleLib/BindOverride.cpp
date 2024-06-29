#include "pch.h"
#include "BindOverride.h"
#include <Psapi.h>
#include <TlHelp32.h>
#include <Shlwapi.h>
#include "MemoryUtility.h"

CBindOverride::FN_WSA_bind CBindOverride::s_pfnOrigWSA_bind;
CBindOverride::FN_IPHLPAPI_GetIpAddrTable CBindOverride::s_pfnOrigIPHLPAPI_GetIpAddrTable;
CBindOverride::FN_WSA_connect CBindOverride::s_pfnOrigWSA_connect;
CBindOverride::FN_WSA_WSAConnect CBindOverride::s_pfnOrigWSA_WSAConnect;

const char* CBindOverride::s_szSpoofIp;

void CBindOverride::Setup(const char* szSpoofIP)
{
	s_szSpoofIp = szSpoofIP;

	CBindOverride::DisableModuleSemaphoreCheck();

	HMODULE hIphlpapi = LoadLibraryA("Iphlpapi.dll");
	HMODULE hWinsock2 = LoadLibraryA("ws2_32.dll");

	printf("hIphlpapi = 0x%p, hWinsock2 = 0x%p\n", hIphlpapi, hWinsock2);

	s_pfnOrigWSA_bind = reinterpret_cast<FN_WSA_bind>(GetProcAddress(hWinsock2, "bind"));
	s_pfnOrigIPHLPAPI_GetIpAddrTable = reinterpret_cast<FN_IPHLPAPI_GetIpAddrTable>(GetProcAddress(hIphlpapi, "GetIpAddrTable"));
	s_pfnOrigWSA_connect = reinterpret_cast<FN_WSA_connect>(GetProcAddress(hWinsock2, "connect"));
	s_pfnOrigWSA_WSAConnect = reinterpret_cast<FN_WSA_WSAConnect>(GetProcAddress(hWinsock2, "WSAConnect"));

	DetourTransactionBegin();
	DetourAttach(&(PVOID&)s_pfnOrigIPHLPAPI_GetIpAddrTable, CBindOverride::MyIPHLPAPI_GetIpAddrTable);
	DetourAttach(&(PVOID&)s_pfnOrigWSA_bind, CBindOverride::MyWSA_bind);
	DetourAttach(&(PVOID&)s_pfnOrigWSA_connect, CBindOverride::MyWSA_connect);
	DetourAttach(&(PVOID&)s_pfnOrigWSA_WSAConnect, CBindOverride::MyWSA_WSAConnect);
	DetourTransactionCommit();
}


bool CBindOverride::DisableModuleSemaphoreCheck()
{
#define ASM_PUSH_32				0x68
#define ASM_SHORT_JMP			0xEB
#define ASM_NOP					0x90

	const char* semaphoreMsg = "cannot create semaphore";

	DWORD dwImgBaseAddr = 0;

	HMODULE hModule = GetModuleHandle(NULL);
	if (hModule == NULL || hModule == INVALID_HANDLE_VALUE)
		return false;

	dwImgBaseAddr = reinterpret_cast<DWORD>(hModule);

	FILE* fp = _wfopen(GetOwnPath().c_str(), L"r");

	if (fp == NULL)
	{
		fclose(fp);
		return false;
	}

	fseek(fp, 0, SEEK_END);
	size_t size = ftell(fp);
	fseek(fp, 0, SEEK_SET);

	char* fileBuffer = new char[size];
	fread(fileBuffer, 1, size, fp);

	unsigned long semaphoreMsgPos = 0;

	char buffer[256];
	//Find semaphore msg addr (rva)
	for (int i = 0; i < size - strlen(semaphoreMsg); i++)
	{
		memcpy(buffer, &fileBuffer[i], strlen(semaphoreMsg));

		if (memcmp(buffer, semaphoreMsg, strlen(semaphoreMsg)) == 0)
		{
			//0x400000 == image base... we need rva !
			semaphoreMsgPos = i + dwImgBaseAddr;
			break;
		}
	}

	if (semaphoreMsgPos == 0)
	{
		delete[] fileBuffer;
		fclose(fp);
		return false;
	}

	DWORD pushSemaphoreMsgPos = 0;

	//Find first semaphore msg push onto stack (0x68)... start at img base
	for (int i = 0; i < size - 4; i++)
	{
		DWORD pushPossibleVal = *(DWORD*)(&fileBuffer[i] + 1);
		if (fileBuffer[i] == ASM_PUSH_32 && pushPossibleVal == semaphoreMsgPos)
		{
			//0x400000 == image base... we need rva !
			pushSemaphoreMsgPos = i + dwImgBaseAddr;
			break;
		}
	}

	//Cleanup... we fetched all required data
	delete[] fileBuffer;
	fclose(fp);

	if (pushSemaphoreMsgPos == 0)
		return false;

	MEMUTIL_WRITE_VALUE(BYTE, pushSemaphoreMsgPos - 2, ASM_SHORT_JMP);

	return true;
}

std::wstring CBindOverride::GetFileNameByProcessId(const DWORD dwProcessID)
{
	HANDLE hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, dwProcessID);
	if (hProcess == INVALID_HANDLE_VALUE || hProcess == NULL)
		return std::wstring();

	wchar_t tmpPath[MAX_PATH];

	if (GetModuleFileNameExW(hProcess, NULL, tmpPath, MAX_PATH) == FALSE)
		return std::wstring();

	CloseHandle(hProcess);
	return std::wstring(tmpPath);
}


std::wstring CBindOverride::GetOwnPath()
{
	return CBindOverride::GetFileNameByProcessId(GetCurrentProcessId());
}

int CBindOverride::RebindSocket(SOCKET socket)
{
	struct sockaddr name;
	int name_len = sizeof(sockaddr);

	int sockName = getsockname(socket, &name, &name_len);
	struct sockaddr_in* sin = (struct sockaddr_in*)(&name);

	if (sockName)
	{
		sin->sin_family = AF_INET;
		sin->sin_port = 0;
		sin->sin_addr.s_addr = inet_addr(s_szSpoofIp);
		memset(sin->sin_zero, 0x00, 8);

		sockName = bind(socket, &name, name_len);
	}
	else if (name_len == sizeof(sockaddr))
	{
		sockName = inet_addr(s_szSpoofIp);
		if (sin->sin_addr.s_addr != inet_addr(s_szSpoofIp))
		{
			sin->sin_addr.s_addr = inet_addr(s_szSpoofIp);
			sockName = bind(socket, &name, sizeof(sockaddr));
		}
	}

	return sockName;
}

int WSAAPI CBindOverride::MyWSA_bind(SOCKET sock, const sockaddr* addr, int namelen)
{
	struct sockaddr_in* sin = (struct sockaddr_in*)(addr);
	char* szAddr = inet_ntoa(sin->sin_addr);

	if (namelen == sizeof(sockaddr))
	{
		if (sin->sin_addr.s_addr != INADDR_LOOPBACK && sin->sin_addr.s_addr != inet_addr(s_szSpoofIp))
		{
			sin->sin_addr.s_addr = inet_addr(s_szSpoofIp);
		}
	}

	return s_pfnOrigWSA_bind(sock, addr, namelen);
}


DWORD WINAPI CBindOverride::MyIPHLPAPI_GetIpAddrTable(PMIB_IPADDRTABLE pIpAddrTable, PULONG pdwSize, BOOL bOrder)
{
	DWORD dwOrigRes = s_pfnOrigIPHLPAPI_GetIpAddrTable(pIpAddrTable, pdwSize, bOrder);

	if (pIpAddrTable != nullptr && pIpAddrTable->dwNumEntries > 0)
	{
		for (int i = 0; i < pIpAddrTable->dwNumEntries; i++)
		{
			//if (pIpAddrTable->table[i].wType != INADDR_LOOPBACK)
			{
				pIpAddrTable->table[i].dwAddr = inet_addr(s_szSpoofIp);
			}
		}
	}

	return dwOrigRes;
}

//Black magic involved, do not touch.
int WSAAPI CBindOverride::MyWSA_connect(SOCKET s, const sockaddr* name, int namelen)
{
	CBindOverride::RebindSocket(s);
	return s_pfnOrigWSA_connect(s, name, namelen);
}

int WSAAPI CBindOverride::MyWSA_WSAConnect(SOCKET s, const sockaddr* name, int namelen, LPWSABUF lpCallerData, LPWSABUF lpCalleeData, LPQOS lpSQOS, LPQOS lpGQOS)
{
	CBindOverride::RebindSocket(s);
	return s_pfnOrigWSA_WSAConnect(s, name, namelen, lpCallerData, lpCalleeData, lpSQOS, lpGQOS);
}