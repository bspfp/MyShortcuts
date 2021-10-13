#pragma once

#include "InteropTypes.h"

class ExplorerBrowser : public IServiceProvider, public ICommDlgBrowser, public IExplorerBrowserEvents {
	wstring homeFolder;
	IExplorerBrowser* explorerBrowser = nullptr;
	DWORD cookie = 0;
	volatile ULONG refCount = 0;
	NavigationCompleteCallback completeCallback = nullptr;
	NavigationFailedCallback failedCallback = nullptr;
	wstring currentFolder;

public:
	ExplorerBrowser() {}
	~ExplorerBrowser();
	ExplorerBrowser(const ExplorerBrowser&) = delete;
	ExplorerBrowser& operator=(const ExplorerBrowser&) = delete;

	HWND Handle();
	HWND Create(HWND hwndParent, FOLDERSETTINGS& folderSettings, EXPLORER_BROWSER_OPTIONS browserOptions, SORTCOLUMN& sortColumn,
				const wchar_t* homeFolder, NavigationCompleteCallback completeCallback, NavigationFailedCallback failedCallback);
	void NavigateToFolder(const wstring& path);
	void NavigateTo(NavigateTarget target);
	void SetFocusToShellView();
	bool PreTranslateMessage(MSG& msg);
	bool OnAppCommand(HWND hwnd, UINT cmd);
	bool IsMyChild(HWND hwndTest);

private:
	// TFunc: bool(IShellView&)
	template<typename TFunc>
	bool ExecuteToShellView(TFunc&& func) {
		if (explorerBrowser != nullptr) {
			IShellView* shellView = nullptr;
			if (SUCCEEDED(explorerBrowser->GetCurrentView(IID_PPV_ARGS(&shellView)))) {
				bool ret = func(*shellView);
				shellView->Release();
				return ret;
			}
		}
		return false;
	}

	// TFunc: bool(IFolderView2&)
	template<typename TFunc>
	bool ExecuteToFolderView2(TFunc&& func) {
		return ExecuteToShellView([&](auto& view) {
			IFolderView2* folderView2 = nullptr;
			if (SUCCEEDED(view.QueryInterface(IID_PPV_ARGS(&folderView2)))) {
				bool ret = func(*folderView2);
				folderView2->Release();
				return ret;
			}
			return false;
		});
	}

public:
#pragma region COM implementaion
	// IUnknown
	IFACEMETHODIMP QueryInterface(REFIID riid, void** ppv);
	IFACEMETHODIMP_(ULONG) AddRef();
	IFACEMETHODIMP_(ULONG) Release();

	// IServiceProvider
	IFACEMETHODIMP QueryService(REFGUID rguid, REFIID riid, void** ppv);

	// ICommDlgBrowser
	IFACEMETHODIMP OnDefaultCommand(IShellView*) { return E_NOTIMPL; }
	IFACEMETHODIMP OnStateChange(IShellView*, ULONG) { return E_NOTIMPL; }
	IFACEMETHODIMP IncludeObject(IShellView*, PCUITEMID_CHILD) { return E_NOTIMPL; }

	// IExplorerBrowserEvents
	IFACEMETHODIMP OnNavigationComplete(PCIDLIST_ABSOLUTE);
	IFACEMETHODIMP OnNavigationFailed(PCIDLIST_ABSOLUTE);
	IFACEMETHODIMP OnNavigationPending(PCIDLIST_ABSOLUTE);
	IFACEMETHODIMP OnViewCreated(IShellView*) { return S_OK; }
#pragma endregion
};