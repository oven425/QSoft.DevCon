using QSoft.DevCon;
using System.Runtime.InteropServices;


Span<char> span = stackalloc char[1024];
string str = new string(span);


Console.ReadLine();
