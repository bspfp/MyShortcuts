#include "DLLInst.h"

unique_ptr<DLLInst> dllInst;

DLLInst::DLLInst() {
	oleInit = SUCCEEDED(OleInitialize(nullptr));
}

DLLInst::~DLLInst() {
	vector<ExplorerBrowser*> explorerBrowsers;
	explorerBrowsers.reserve(explorerBrowserMap.size());
	{
		scoped_lock lock(lockExplorerBrowserMap);
		for (auto& [k, v] : explorerBrowserMap)
			explorerBrowsers.emplace_back(v);
		explorerBrowserMap.clear();
	}

	for (auto& v : explorerBrowsers)
		v->Release();
	explorerBrowsers.clear();

	if (oleInit) {
		OleUninitialize();
		oleInit = false;
	}
}

HWND DLLInst::CreateExplorerBrowser(HWND hwndParent, FOLDERSETTINGS& folderSettings, EXPLORER_BROWSER_OPTIONS browserOptions, SORTCOLUMN& sortColumn,
									const wchar_t* homeFolder, NavigationCompleteCallback completeCallback, NavigationFailedCallback failedCallback) {
	unique_ptr<ExplorerBrowser> explorerBrowser(new ExplorerBrowser());

	if (auto hwndControl = explorerBrowser->Create(hwndParent, folderSettings, browserOptions, sortColumn, homeFolder, completeCallback, failedCallback);
		hwndControl != nullptr) {
		explorerBrowser->AddRef();

		{
			scoped_lock lock(lockExplorerBrowserMap);
			explorerBrowserMap[hwndControl] = explorerBrowser.release();
		}

		return hwndControl;
	}

	return nullptr;
}

void DLLInst::DestroyExplorerBrowser(HWND hwnd) {
	ExplorerBrowser* explorerBrowser = nullptr;
	{
		scoped_lock lock(lockExplorerBrowserMap);
		if (auto it = explorerBrowserMap.find(hwnd); it != explorerBrowserMap.end()) {
			explorerBrowser = it->second;
			explorerBrowserMap.erase(it);
		}
	}

	if (explorerBrowser != nullptr) {
		ShowWindow(hwnd, SW_HIDE);
		explorerBrowser->Release();
	}
}

void DLLInst::SetFocusToShellView(HWND hwnd) {
	ExplorerBrowser* explorerBrowser = nullptr;
	{
		scoped_lock lock(lockExplorerBrowserMap);
		if (auto it = explorerBrowserMap.find(hwnd); it != explorerBrowserMap.end()) {
			explorerBrowser = it->second;
			explorerBrowser->AddRef();
		}
	}

	if (explorerBrowser != nullptr) {
		explorerBrowser->SetFocusToShellView();
		explorerBrowser->Release();
	}
}

bool DLLInst::PreTranslateMessage(HWND hwnd, MSG& msg) {
	ExplorerBrowser* explorerBrowser = nullptr;
	{
		scoped_lock lock(lockExplorerBrowserMap);
		if (auto it = explorerBrowserMap.find(hwnd); it != explorerBrowserMap.end()) {
			explorerBrowser = it->second;
			explorerBrowser->AddRef();
		}
	}

	if (explorerBrowser != nullptr) {
		auto ret = explorerBrowser->PreTranslateMessage(msg);
		explorerBrowser->Release();
		return ret;
	}

	return false;
}

bool DLLInst::OnAppCommand(HWND hwnd, HWND child, UINT cmd) {
	ExplorerBrowser* explorerBrowser = nullptr;
	{
		scoped_lock lock(lockExplorerBrowserMap);
		if (auto it = explorerBrowserMap.find(hwnd); it != explorerBrowserMap.end()) {
			explorerBrowser = it->second;
			explorerBrowser->AddRef();
		}
	}

	if (explorerBrowser != nullptr) {
		auto ret = explorerBrowser->OnAppCommand(child, cmd);
		explorerBrowser->Release();
		return ret;
	}

	return false;
}

void DLLInst::NavigateToFolder(HWND hwnd, const wchar_t* path) {
	ExplorerBrowser* explorerBrowser = nullptr;
	{
		scoped_lock lock(lockExplorerBrowserMap);
		if (auto it = explorerBrowserMap.find(hwnd); it != explorerBrowserMap.end()) {
			explorerBrowser = it->second;
			explorerBrowser->AddRef();
		}
	}

	if (explorerBrowser != nullptr) {
		explorerBrowser->NavigateToFolder(path);
		explorerBrowser->Release();
	}
}

void DLLInst::NavigateTo(HWND hwnd, NavigateTarget target) {
	ExplorerBrowser* explorerBrowser = nullptr;
	{
		scoped_lock lock(lockExplorerBrowserMap);
		if (auto it = explorerBrowserMap.find(hwnd); it != explorerBrowserMap.end()) {
			explorerBrowser = it->second;
			explorerBrowser->AddRef();
		}
	}

	if (explorerBrowser != nullptr) {
		explorerBrowser->NavigateTo(target);
		explorerBrowser->Release();
	}
}
