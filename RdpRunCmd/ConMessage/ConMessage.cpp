
#include <iostream>

#include <windows.h>
#include <time.h>

using namespace std;

void simulate();
void simulate2();


int main()
{
	time_t now = time(0);
	char* dt = ctime(&now);
	cout << "本地日期和时间：" << dt << endl;
	Sleep(2000);

	simulate();

	return 0;
}


void simulate() {
	//work for those use message on loop;
	//not work for Dx11...
	auto hwnd = (HWND)0x200954;
	cout << "sleep" << endl;
	

	PostMessage(hwnd, WM_KEYDOWN, 0xA2, 0x1D0001);
	PostMessage(hwnd, WM_KEYDOWN, 0x11, 0x1D0001);
	PostMessage(hwnd, WM_KEYDOWN, 0x56, 0x2F0001);

	//Sleep(50);
	

	PostMessage(hwnd, WM_KEYUP, 0xA2,0xFFFFFFFF801D0001);
	PostMessage(hwnd, WM_KEYUP, 0x56,0xFFFFFFFF802F0001);
	PostMessage(hwnd, WM_KEYUP, 0x11,0xC01D0001);
	
}


void simulate2() {
	auto hWnd = (HWND)0x200954;

	keybd_event(VK_CONTROL, 0, 0, 0);
	PostMessage(hWnd, WM_KEYDOWN, 'V', 0x2F0001);
	PostMessage(hWnd, WM_KEYUP, 'V', 0xFFFFFFFF802F0001);
	keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
}




