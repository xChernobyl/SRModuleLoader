// MultiModuleLoader.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include "ProcUtility.h"
#include "Injector.h"

//-1 -> invalid arg count
//-2 -> failed to create suspended proc
//-3 -> failed to inject library
//1 -> success
int wmain(int argc, wchar_t* argv[])
{
    if (argc != 4)
    {
        printf("Invalid argument count, must be 4 (full proc path + dll path).\n");
        return -1;
    }

    PROCESS_INFORMATION pi = { 0 };

    if (!CProcUtility::CreateSuspendedProcess(argv[1], &pi))
        return -2;

    std::wstring tmpW(argv[3]);
    std::string tmpA(tmpW.begin(), tmpW.end());

    if (!CInjector::InjectLibrary(pi.dwProcessId, argv[1], argv[2], tmpA.c_str()))
        return -3;

    //Now after we have the dll loaded we can resume the target process.
    ResumeThread(pi.hThread);

    //getchar();
    return pi.dwProcessId;
}