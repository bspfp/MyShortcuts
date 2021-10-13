#pragma once

#include "InteropTypes.h"
#include "ExplorerBrowser.h"

class DLLInst {
public:
	DLLInst();
	~DLLInst();

	HWND CreateExplorerBrowser(HWND hwndParent, FOLDERSETTINGS& folderSettings, EXPLORER_BROWSER_OPTIONS browserOptions, SORTCOLUMN& sortColumn,
							   const wchar_t* homeFolder, NavigationCompleteCallback completeCallback, NavigationFailedCallback failedCallback);
	void DestroyExplorerBrowser(HWND hwnd);
	void SetFocusToShellView(HWND hwnd);
	bool PreTranslateMessage(HWND hwnd, MSG& msg);
	bool OnAppCommand(HWND hwnd, HWND child, UINT cmd);
	void NavigateToFolder(HWND hwnd, const wchar_t* path);
	void NavigateTo(HWND hwnd, NavigateTarget target);

private:
	// OLE √ ±‚»≠
	bool oleInit = false;

	// ExplorerBrowser
	mutex lockExplorerBrowserMap;
	unordered_map<HWND, ExplorerBrowser*> explorerBrowserMap;
};

extern unique_ptr<DLLInst> dllInst;