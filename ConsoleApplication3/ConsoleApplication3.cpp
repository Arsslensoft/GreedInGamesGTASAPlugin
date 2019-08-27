// ConsoleApplication3.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <assert.h>
#include <string.h>
#include <string>
#include <stdlib.h> 
#include <stdio.h> 
#include <assert.h>
char* substring(const char *text, int start, int stop)
{ 
	char* res = "ABC";
	char buffer[10];
	//sprintf_s(buffer, "%.*s\n", stop - start, &text[start]);
	
	strncpy_s(buffer,text+1,3);
	return  buffer;
}
std::string SubstringOfCString(const char *cstr,    size_t start, size_t length)
{
    assert(start + length <= strlen(cstr));
    return std::string(cstr + start, length);
}


int _tmain(int argc, _TCHAR* argv[])
{
const char* from = "12345678";
if(SubstringOfCString(from, 1, 3) == "234")
printf("%s DONE\n", SubstringOfCString(from, 1, 3).c_str());
int x = std::stoi(SubstringOfCString(from, 1, 3));
printf("%d",x);
  //printf("%s DONE", substring(from, 1, 3));

	getchar();
	return 0;
}

