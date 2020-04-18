
#include <iostream>

#include <windows.h>
#include <time.h>

using namespace std;

void simulate();


int main()
{
	time_t now = time(0);
	char* dt = ctime(&now);
	cout << "本地日期和时间：" << dt << endl;

	simulate();

	return 0;
}


void simulate() {
	//work for those use message on loop;
	//not work for Dx11...
	auto hwnd = (HWND)0x5b0e9c;


	auto res = SendMessage(hwnd, 0x0104, 0x11, 0x20380001);//0x11 == VK_CONTROL == ALT键
	SendMessage(hwnd, 0x0104, (int)'F', 0x20210001);//

	SendMessage(hwnd, 0x0106, (int)'f', 0x20210001);//
	SendMessage(hwnd, 0x0105, (int)'F', 0xE0210001);//
	SendMessage(hwnd, 0x0105, 0x11, 0xC0380001);//
}






