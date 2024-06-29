#pragma once

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS

// Windows Header Files
#include <windows.h>
#include <iostream>
#include <string>
#include <WinSock2.h>
#include <iphlpapi.h>
#include <WS2tcpip.h>
#include <fstream>
#include <filesystem>
#include "detours.h"

#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib, "Iphlpapi.lib")
#pragma comment(lib, "detours.lib")