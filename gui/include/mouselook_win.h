// Mouse look support: Windows raw-input helpers kept out of the big Qt files so
// they don't have to include <windows.h>.
#ifndef CHIAKI_GUI_MOUSELOOK_WIN_H
#define CHIAKI_GUI_MOUSELOOK_WIN_H

#include <cstdint>

// Button transition flags reported with each raw mouse packet (same bit layout
// as Windows' RI_MOUSE_* flags).
enum MouseLookButtonFlag : uint16_t
{
	MOUSELOOK_LEFT_DOWN   = 0x0001,
	MOUSELOOK_LEFT_UP     = 0x0002,
	MOUSELOOK_RIGHT_DOWN  = 0x0004,
	MOUSELOOK_RIGHT_UP    = 0x0008,
	MOUSELOOK_MIDDLE_DOWN = 0x0010,
	MOUSELOOK_MIDDLE_UP   = 0x0020,
	MOUSELOOK_X1_DOWN     = 0x0040,
	MOUSELOOK_X1_UP       = 0x0080,
	MOUSELOOK_X2_DOWN     = 0x0100,
	MOUSELOOK_X2_UP       = 0x0200,
};

// Registers the window for raw mouse input. Safe to call more than once.
bool MouseLookRegisterRawInput(void *hwnd);

// If message (a Windows MSG*) is a relative raw mouse packet, fills in the
// movement and button flags and returns true.
bool MouseLookParseRawInput(void *message, int *dx, int *dy, uint16_t *button_flags);

// Confines the cursor to the window's client area (on = true) or releases it.
void MouseLookClipCursor(void *hwnd, bool on);

// Short, non-blocking sound: one when a mode turns on, another when it turns off.
void MouseLookBeep(bool on);

#endif
