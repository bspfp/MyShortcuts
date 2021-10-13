#pragma once

/*
internal delegate void NavigationCompleteCallback(string parsingName, string editName, string normalName, bool isFileSystemFolder);
internal delegate void NavigationFailedCallback(string parsingName, string editName, string normalName, bool isFileSystemFolder);
*/
using NavigationCompleteCallback = void(CALLBACK*)(const wchar_t* parsingName, const wchar_t* editName, const wchar_t* normalName, bool isFileSystemFolder);
using NavigationFailedCallback = void(CALLBACK*)(const wchar_t* parsingName, const wchar_t* editName, const wchar_t* normalName, bool isFileSystemFolder);

enum class NavigateTarget {
    Backward,
    Forward,
    Up,
    Home
};