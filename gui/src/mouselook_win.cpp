#include "mouselook_win.h"

#ifdef _WIN32

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif
#ifndef NOMINMAX
#define NOMINMAX
#endif
#include <windows.h>

bool MouseLookRegisterRawInput(void *hwnd)
{
	RAWINPUTDEVICE rid;
	rid.usUsagePage = 0x01; // generic desktop
	rid.usUsage = 0x02;     // mouse
	rid.dwFlags = 0;        // only while our window is in the foreground; legacy messages still flow
	rid.hwndTarget = static_cast<HWND>(hwnd);
	return RegisterRawInputDevices(&rid, 1, sizeof(rid)) == TRUE;
}

bool MouseLookParseRawInput(void *message, int *dx, int *dy, uint16_t *button_flags)
{
	MSG *msg = static_cast<MSG *>(message);
	if(!msg || msg->message != WM_INPUT)
		return false;

	RAWINPUT raw;
	UINT size = sizeof(raw);
	if(GetRawInputData(reinterpret_cast<HRAWINPUT>(msg->lParam), RID_INPUT, &raw, &size, sizeof(RAWINPUTHEADER)) == (UINT)-1)
		return false;
	if(raw.header.dwType != RIM_TYPEMOUSE)
		return false;

	const RAWMOUSE &m = raw.data.mouse;
	if(m.usFlags & MOUSE_MOVE_ABSOLUTE)
	{
		// Tablets / remote desktop report absolute positions; only take the buttons.
		*dx = 0;
		*dy = 0;
	}
	else
	{
		*dx = m.lLastX;
		*dy = m.lLastY;
	}
	*button_flags = m.usButtonFlags & 0x03ff;
	return true;
}

void MouseLookClipCursor(void *hwnd, bool on)
{
	if(!on || !hwnd)
	{
		ClipCursor(nullptr);
		return;
	}
	HWND h = static_cast<HWND>(hwnd);
	RECT r;
	if(!GetClientRect(h, &r))
		return;
	POINT tl = { r.left, r.top };
	POINT br = { r.right, r.bottom };
	ClientToScreen(h, &tl);
	ClientToScreen(h, &br);
	// Keep the cursor away from the very edges so it can't click other windows' borders.
	RECT clip = { tl.x + 2, tl.y + 2, br.x - 2, br.y - 2 };
	if(clip.right <= clip.left || clip.bottom <= clip.top)
		clip = { tl.x, tl.y, br.x, br.y };
	ClipCursor(&clip);
}

void MouseLookBeep(bool on)
{
	// MessageBeep plays asynchronously, so it never stalls input
	MessageBeep(on ? MB_ICONASTERISK : MB_OK);
}

#else

bool MouseLookRegisterRawInput(void *) { return false; }
bool MouseLookParseRawInput(void *, int *, int *, uint16_t *) { return false; }
void MouseLookClipCursor(void *, bool) {}
void MouseLookBeep(bool) {}

#endif
