#pragma once

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <Windows.h>
#include <Ole2.h>
#include <UserEnv.h>
#include <shellapi.h>
#include <ShlObj.h>
#include <objbase.h>
#include <Shlwapi.h>
#include <KnownFolders.h>
#include <propkey.h>

#pragma comment(lib, "Ole32.lib")
#pragma comment(lib, "Userenv.lib")
#pragma comment(lib, "Shell32.lib")
#pragma comment(lib, "Shlwapi.lib")

#include <cstdint>
#include <cstdlib>
#include <cstdio>
#include <string>
#include <memory>
#include <array>
#include <utility>
#include <type_traits>
#include <filesystem>
#include <list>
#include <unordered_map>
#include <mutex>

using namespace std;
