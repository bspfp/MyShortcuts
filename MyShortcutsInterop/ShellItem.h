#pragma once

class ShellItem {
	IShellItem* shellItem = nullptr;

public:
	IShellItem* Item() { return shellItem; }

	wstring ParsingName();
	wstring EditName();
	wstring NormalName();
	bool IsFileSystemFolder();

	ShellItem() {}
	ShellItem(const ShellItem&) = delete;
	ShellItem& operator=(const ShellItem&) = delete;
	~ShellItem();

	bool FromFolderPath(const wstring& path);
	bool FromIDList(PCIDLIST_ABSOLUTE pidl);

private:
	void Release();
};
