#pragma once

#include "DLLInst.h"
#include "InteropTypes.h"

#ifdef __cplusplus
extern "C" {
#endif
	/*
	[DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
	internal static extern IntPtr CreateExplorerBrowser(IntPtr hwndParent, ref FOLDERSETTINGS folderSettings, EXPLORER_BROWSER_OPTIONS browserOptions, ref SORTCOLUMN sortColumn,
														string homeFolder, NavigationCompleteCallback completeCallback, NavigationFailedCallback failedCallback);
	*/
	__declspec(dllexport) HWND __cdecl CreateExplorerBrowser(HWND hwndParent, FOLDERSETTINGS& folderSettings, EXPLORER_BROWSER_OPTIONS browserOptions, SORTCOLUMN& sortColumn,
															 const wchar_t* homeFolder, NavigationCompleteCallback completeCallback, NavigationFailedCallback failedCallback) {
		if (dllInst != nullptr)
			return dllInst->CreateExplorerBrowser(hwndParent, folderSettings, browserOptions, sortColumn, homeFolder, completeCallback, failedCallback);
		return nullptr;
	}

	/*
	[DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
	internal static extern void DestroyExplorerBrowser(IntPtr hwnd);
	*/
	__declspec(dllexport) void __cdecl DestroyExplorerBrowser(HWND hwnd) {
		if (dllInst != nullptr)
			dllInst->DestroyExplorerBrowser(hwnd);
	}

	/*
	[DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
	internal static extern void BringToTop(IntPtr hwndMain);
	*/
	__declspec(dllexport)  void __cdecl BringToTop(HWND hwndMain) {
		if (hwndMain == nullptr)
			return;

		HWND hwndForeground = GetForegroundWindow();
		if (hwndForeground == hwndMain) {
			BringWindowToTop(hwndMain);
			return;
		}

		WINDOWPLACEMENT wp = {};
		wp.length = sizeof(wp);
		GetWindowPlacement(hwndMain, &wp);

		auto tidTarget = GetWindowThreadProcessId(hwndMain, nullptr);
		auto tidForeground = GetWindowThreadProcessId(hwndForeground, nullptr);
		if (tidTarget == tidForeground || AttachThreadInput(GetCurrentThreadId(), tidForeground, TRUE)) {
			if (wp.showCmd == SW_SHOWMINIMIZED)
				ShowWindow(hwndMain, SW_RESTORE);
			else if (wp.showCmd == SW_SHOWMAXIMIZED)
				ShowWindow(hwndMain, SW_SHOWMAXIMIZED);
			else
				ShowWindow(hwndMain, SW_SHOWNORMAL);
			SetForegroundWindow(hwndMain);
			BringWindowToTop(hwndMain);
			if (tidTarget != tidForeground)
				AttachThreadInput(GetCurrentThreadId(), tidForeground, FALSE);
		}
	}

	/*
	[DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
	internal static extern void SetFocusToShellView(IntPtr hwnd);
	*/
	__declspec(dllexport) void __cdecl SetFocusToShellView(HWND hwnd) {
		if (dllInst != nullptr)
			dllInst->SetFocusToShellView(hwnd);
	}

	/*
	[DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
	internal static extern bool PreTranslateMessage(IntPtr hwnd, ref MSG msg);
	*/
	__declspec(dllexport) bool __cdecl PreTranslateMessage(HWND hwnd, MSG& msg) {
		if (dllInst != nullptr)
			return dllInst->PreTranslateMessage(hwnd, msg);
		return false;
	}

	/*
	[DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
	internal static extern bool OnAppCommand(IntPtr hwnd, IntPtr child, uint cmd);
	*/
	__declspec(dllexport) bool __cdecl OnAppCommand(HWND hwnd, HWND child, UINT cmd) {
		if (dllInst != nullptr)
			return dllInst->OnAppCommand(hwnd, child, cmd);
		return false;
	}

	/*
	[DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
	internal static extern void NavigateToFolder(IntPtr hwnd, string path);
	*/
	__declspec(dllexport) void __cdecl NavigateToFolder(HWND hwnd, const wchar_t* path) {
		if (dllInst != nullptr)
			dllInst->NavigateToFolder(hwnd, path);
	}

	/*
	[DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
	internal static extern void BringToBottom(IntPtr hwndMain);
	*/
	__declspec(dllexport) void __cdecl BringToBottom(HWND hwndMain) {
		if (hwndMain == nullptr)
			return;

		SetWindowPos(hwndMain, HWND_BOTTOM, 0, 0, 0, 0, SWP_ASYNCWINDOWPOS | SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
	}

	/*
	[DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
    internal static extern void NavigateTo(IntPtr hwnd, NavigateTarget target);
	*/
	__declspec(dllexport) void __cdecl NavigateTo(HWND hwnd, NavigateTarget target) {
		if (dllInst != nullptr)
			dllInst->NavigateTo(hwnd, target);
	}

#ifdef __cplusplus
}
#endif