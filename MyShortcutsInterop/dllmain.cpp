#include "DLLInst.h"

BOOL APIENTRY DllMain(HMODULE hModule,DWORD  reason,LPVOID) {
	switch (reason) {
	case DLL_PROCESS_ATTACH:
		dllInst.reset(new DLLInst());
		break;

	case DLL_PROCESS_DETACH:
		if (dllInst != nullptr)
			dllInst.reset();
		break;
	}
	return TRUE;
}
