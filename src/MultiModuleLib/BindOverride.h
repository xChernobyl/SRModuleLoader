#pragma once
#include "pch.h"

class CBindOverride
{
private:
	using FN_WSA_bind = int (WSAAPI*)(SOCKET sock, const sockaddr* addr, int namelen);
	using FN_IPHLPAPI_GetIpAddrTable = DWORD(WINAPI*)(PMIB_IPADDRTABLE pIpAddrTable, PULONG pdwSize, BOOL bOrder);
	using FN_WSA_connect = int (WSAAPI*)(SOCKET s, const sockaddr* name, int namelen);
	using FN_WSA_WSAConnect = int (WSAAPI*)(SOCKET s, const sockaddr* name, int namelen, LPWSABUF lpCallerData, LPWSABUF lpCalleeData, LPQOS lpSQOS, LPQOS lpGQOS);

	static FN_WSA_bind s_pfnOrigWSA_bind;
	static FN_IPHLPAPI_GetIpAddrTable s_pfnOrigIPHLPAPI_GetIpAddrTable;
	static FN_WSA_connect s_pfnOrigWSA_connect;
	static FN_WSA_WSAConnect s_pfnOrigWSA_WSAConnect;

	static const char* s_szSpoofIp;


	static int WSAAPI MyWSA_bind(SOCKET sock, const sockaddr* addr, int namelen);
	static DWORD WINAPI MyIPHLPAPI_GetIpAddrTable(PMIB_IPADDRTABLE pIpAddrTable, PULONG pdwSize, BOOL bOrder);
	static hostent* WSAAPI MyWSA_gethostbyname(const char* szName);
	static unsigned long WSAAPI MyWSA_inet_addr(const char* szAddr);
	static int WSAAPI MyWSA_connect(SOCKET s, const sockaddr* name, int namelen);
	static int WSAAPI MyWSA_WSAConnect(SOCKET s, const sockaddr* name, int namelen, LPWSABUF lpCallerData, LPWSABUF lpCalleeData, LPQOS lpSQOS, LPQOS lpGQOS);

	static std::wstring GetFileNameByProcessId(const DWORD dwProcessID);
	static std::wstring GetOwnPath();
	static int RebindSocket(SOCKET socket);

public:
	static void Setup(const char* szSpoofIP);
	static bool DisableModuleSemaphoreCheck();
};
