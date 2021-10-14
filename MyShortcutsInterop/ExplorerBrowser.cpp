#include "ExplorerBrowser.h"
#include "ShellItem.h"

ExplorerBrowser::~ExplorerBrowser() {
	if (explorerBrowser != nullptr) {
		if (cookie != 0) {
			explorerBrowser->Unadvise(cookie);
			cookie = 0;
		}
		explorerBrowser->Destroy();
		explorerBrowser->Release();
		explorerBrowser = nullptr;
	}
}

HWND ExplorerBrowser::Handle() {
	HWND ret = nullptr;
	if (explorerBrowser != nullptr) {
		IOleWindow* pio = nullptr;
		if (SUCCEEDED(explorerBrowser->QueryInterface(IID_PPV_ARGS(&pio)))) {
			HWND hwnd;
			if (SUCCEEDED(pio->GetWindow(&hwnd)))
				ret = hwnd;
			pio->Release();
		}
	}
	return ret;
}

HWND ExplorerBrowser::Create(HWND hwndParent, FOLDERSETTINGS& folderSettings, EXPLORER_BROWSER_OPTIONS browserOptions, SORTCOLUMN& sortColumn,
							 const wchar_t* homeFolder, NavigationCompleteCallback completeCallback, NavigationFailedCallback failedCallback) {
	this->homeFolder = homeFolder;

	RECT rect = { 0,0,0,0 };
	if (SUCCEEDED(CoCreateInstance(CLSID_ExplorerBrowser, nullptr, CLSCTX_INPROC, IID_PPV_ARGS(&explorerBrowser)))) {
		IUnknown_SetSite(explorerBrowser, (IServiceProvider*)this);
		if (SUCCEEDED(explorerBrowser->Advise((IExplorerBrowserEvents*)this, &cookie))) {
			if (SUCCEEDED(explorerBrowser->Initialize(hwndParent, &rect, &folderSettings))) {
				if (SUCCEEDED(explorerBrowser->SetOptions(browserOptions))) {
					ExecuteToFolderView2([&](IFolderView2& view) { view.SetSortColumns(&sortColumn, 1); return true; });
					this->completeCallback = completeCallback;
					this->failedCallback = failedCallback;
					return Handle();
				}
				explorerBrowser->Destroy();
			}
			explorerBrowser->Unadvise(cookie);
			cookie = 0;
		}
		explorerBrowser->Release();
		explorerBrowser = nullptr;
	}
	return nullptr;
}

void ExplorerBrowser::NavigateToFolder(const wstring& path) {
	if (explorerBrowser != nullptr) {
		ShellItem shellItem;
		if (shellItem.FromFolderPath(path)) {
			explorerBrowser->BrowseToObject(shellItem.Item(), 0);
		}
	}
}

void ExplorerBrowser::NavigateTo(NavigateTarget target) {
	if (explorerBrowser != nullptr) {
		switch (target) {
		case NavigateTarget::Backward:
			explorerBrowser->BrowseToIDList(nullptr, SBSP_NAVIGATEBACK);
			break;

		case NavigateTarget::Forward:
			explorerBrowser->BrowseToIDList(nullptr, SBSP_NAVIGATEFORWARD);
			break;

		case NavigateTarget::Up:
			explorerBrowser->BrowseToIDList(nullptr, SBSP_PARENT);
			break;

		case NavigateTarget::Home:
			if (ShellItem shellItem; shellItem.FromFolderPath(homeFolder)) {
				explorerBrowser->BrowseToObject(shellItem.Item(), 0);
			}
			else {
				PIDLIST_ABSOLUTE pidl = nullptr;
				if (SUCCEEDED(SHGetKnownFolderIDList(FOLDERID_Desktop, 0, nullptr, &pidl))) {
					explorerBrowser->BrowseToIDList(pidl, SBSP_ABSOLUTE);
					ILFree(pidl);
				}
			}
			break;

		default:
			break;
		}
	}
}

void ExplorerBrowser::SetFocusToShellView() {
	ExecuteToShellView([](auto& view) {
		view.UIActivate(SVUIA_ACTIVATE_FOCUS);
		return true;
	});
}

bool ExplorerBrowser::PreTranslateMessage(MSG& msg) {
	if (explorerBrowser != nullptr) {
		if (WM_KEYFIRST <= msg.message && msg.message <= WM_KEYLAST) {
			if (IsMyChild(msg.hwnd)) {
				bool bret = false;
				IInputObject* pIO = nullptr;
				if (SUCCEEDED(explorerBrowser->QueryInterface(IID_PPV_ARGS(&pIO)))) {
					bret = (pIO->HasFocusIO() == S_OK) && (pIO->TranslateAcceleratorIO(&msg) == S_OK);
					pIO->Release();
				}
				return bret;
			}
		}
	}

	return false;
}

bool ExplorerBrowser::OnAppCommand(HWND hwnd, UINT cmd) {
	if (!IsMyChild(hwnd))
		return false;

	switch (cmd) {
	case APPCOMMAND_BROWSER_BACKWARD:
		NavigateTo(NavigateTarget::Backward);
		return true;

	case APPCOMMAND_BROWSER_FORWARD:
		NavigateTo(NavigateTarget::Forward);
		return true;

	case APPCOMMAND_BROWSER_HOME:
		NavigateTo(NavigateTarget::Home);
		return true;

	case APPCOMMAND_BROWSER_REFRESH:
		ExecuteToShellView([&](IShellView& view) { view.Refresh(); return true; });
		return true;

	default:
		break;
	}

	return false;
}

bool ExplorerBrowser::IsMyChild(HWND hwndTest) {
	return !!IsChild(Handle(), hwndTest);
}

IFACEMETHODIMP ExplorerBrowser::QueryInterface(REFIID riid, void** ppv) {
	static const QITAB qit[] = {
		QITABENT(ExplorerBrowser, IServiceProvider),
		QITABENT(ExplorerBrowser, ICommDlgBrowser),
		{ 0 },
	};
	return QISearch(this, qit, riid, ppv);
}

ULONG __stdcall ExplorerBrowser::AddRef() {
	return InterlockedIncrement(&refCount);
}

ULONG __stdcall ExplorerBrowser::Release() {
	auto ret = InterlockedDecrement(&refCount);
	if (ret == 0)
		delete this;
	return ret;
}

IFACEMETHODIMP ExplorerBrowser::QueryService(REFGUID rguid, REFIID riid, void** ppv) {
	*ppv = nullptr;
	if (rguid == SID_SExplorerBrowserFrame)
		return QueryInterface(riid, ppv);
	return E_NOINTERFACE;
}

IFACEMETHODIMP ExplorerBrowser::OnNavigationComplete(PCIDLIST_ABSOLUTE pidl) {
	ShellItem shellItem;
	if (shellItem.FromIDList(pidl) && completeCallback != nullptr)
		completeCallback(shellItem.ParsingName().c_str(), shellItem.EditName().c_str(), shellItem.NormalName().c_str(), shellItem.IsFileSystemFolder());

	return S_OK;
}

IFACEMETHODIMP ExplorerBrowser::OnNavigationFailed(PCIDLIST_ABSOLUTE pidl) {
	ShellItem shellItem;
	if (shellItem.FromIDList(pidl)) {
		currentFolder = shellItem.ParsingName();
		if (failedCallback != nullptr)
			failedCallback(shellItem.ParsingName().c_str(), shellItem.EditName().c_str(), shellItem.NormalName().c_str(), shellItem.IsFileSystemFolder());
	}

	return S_OK;
}

IFACEMETHODIMP ExplorerBrowser::OnNavigationPending(PCIDLIST_ABSOLUTE pidl) {
	ShellItem shellItem;
	if (shellItem.FromIDList(pidl)) {
		auto requested = shellItem.ParsingName();
		if (_wcsicmp(currentFolder.c_str(), requested.c_str()) == 0)
			return E_FAIL;
	}

	return S_OK;
}
