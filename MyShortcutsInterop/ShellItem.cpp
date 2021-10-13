#include "ShellItem.h"

wstring ShellItem::ParsingName() {
	wstring ret;
	wchar_t* name = nullptr;
	if (shellItem != nullptr && SUCCEEDED(shellItem->GetDisplayName(SIGDN_DESKTOPABSOLUTEPARSING, &name))) {
		ret = name;
		CoTaskMemFree(name);
	}
	return ret;
}

wstring ShellItem::EditName() {
	wstring ret;
	wchar_t* name = nullptr;
	if (shellItem != nullptr && SUCCEEDED(shellItem->GetDisplayName(SIGDN_DESKTOPABSOLUTEEDITING, &name))) {
		ret = name;
		CoTaskMemFree(name);
	}
	return ret;
}

wstring ShellItem::NormalName() {
	wstring ret;
	wchar_t* name = nullptr;
	if (shellItem != nullptr && SUCCEEDED(shellItem->GetDisplayName(SIGDN_NORMALDISPLAY, &name))) {
		ret = name;
		CoTaskMemFree(name);
	}
	return ret;
}

bool ShellItem::IsFileSystemFolder() {
	wchar_t* name = nullptr;
	SFGAOF flag = 0;
	if (shellItem != nullptr && SUCCEEDED(shellItem->GetAttributes(SFGAO_FILESYSTEM | SFGAO_FOLDER | SFGAO_STREAM, &flag)))
		return ((flag & (SFGAO_FILESYSTEM | SFGAO_FOLDER | SFGAO_STREAM)) == (SFGAO_FILESYSTEM | SFGAO_FOLDER));
	return false;
}

ShellItem::~ShellItem() {
	Release();
}

bool ShellItem::FromFolderPath(const wstring& path) {
	IShellItem* newShellItem = nullptr;

	wchar_t fullpath[65536] = {};
	if (!GetFullPathNameW(path.c_str(), DWORD(size(fullpath)), fullpath, nullptr))
		return false;
	if (!filesystem::exists(fullpath) || !filesystem::is_directory(fullpath))
		return false;
	if (SUCCEEDED(SHCreateItemFromParsingName(fullpath, nullptr, IID_PPV_ARGS(&newShellItem)))) {
		Release();
		shellItem = newShellItem;
		return true;
	}
	if (SUCCEEDED(SHCreateItemInKnownFolder(FOLDERID_Desktop, 0, nullptr, IID_PPV_ARGS(&newShellItem)))) {
		Release();
		shellItem = newShellItem;
		return true;
	}
	return true;
}

bool ShellItem::FromIDList(PCIDLIST_ABSOLUTE pidl) {
	IShellItem* newShellItem = nullptr;
	if (SUCCEEDED(SHCreateItemFromIDList(pidl, IID_PPV_ARGS(&newShellItem)))) {
		Release();
		shellItem = newShellItem;
		return true;
	}
	return false;
}

void ShellItem::Release() {
	if (shellItem != nullptr) {
		shellItem->Release();
		shellItem = nullptr;
	}
}
