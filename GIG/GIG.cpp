#define WIN32_LEAN_AND_MEAN

#include "../SDK/plugin.h"

#include <string>
#include <vector>
#include <string.h>
#include <assert.h>
#include <cstdlib>
#include <ctime> 
#include <WinSock2.h>
#include <WS2tcpip.h>
#include <iostream>
 
// link with Ws2_32.lib
#pragma comment(lib, "Ws2_32.lib")
 
#define DEFAULT_PORT "6667" 
#define DEFAULT_BUFFER_LENGTH    512
 

using namespace std;
typedef void
    (*logprintf_t)(char* format, ...)
;

logprintf_t
    logprintf
;

void
    **ppPluginData
;

extern void
    *pAMXFunctions
;

class Client {
public:
    Client(char* servername)
    {
        szServerName = servername;
        ConnectSocket = INVALID_SOCKET;
    }
 
    bool Start() {
        WSADATA wsaData;
 
        // Initialize Winsock
        int iResult = WSAStartup(MAKEWORD(2,2), &wsaData);
        if(iResult != 0)
        {
            printf("WSAStartup failed: %d\n", iResult);
            return false;
        }
 
        struct addrinfo    *result = NULL,
                        *ptr = NULL,
                        hints;
 
        ZeroMemory(&hints, sizeof(hints));
        hints.ai_family = AF_UNSPEC;        
        hints.ai_socktype = SOCK_STREAM;    
        hints.ai_protocol = IPPROTO_TCP;
 
        // Resolve the server address and port
        iResult = getaddrinfo(szServerName, DEFAULT_PORT, &hints, &result);
        if (iResult != 0)
        {
            printf("getaddrinfo failed: %d\n", iResult);
            WSACleanup();
            return false;
        }
 
        ptr = result;
 
        // Create a SOCKET for connecting to server
        ConnectSocket = socket(ptr->ai_family, ptr->ai_socktype, ptr->ai_protocol);
 
        if (ConnectSocket == INVALID_SOCKET)
        {
            printf("Error at socket(): %d\n", WSAGetLastError());
            freeaddrinfo(result);
            WSACleanup();
            return false;
        }
 
        // Connect to server
        iResult = connect(ConnectSocket, ptr->ai_addr, (int)ptr->ai_addrlen);
 
        if (iResult == SOCKET_ERROR)
        {
            closesocket(ConnectSocket);
            ConnectSocket = INVALID_SOCKET;
        }
 
        freeaddrinfo(result);
 
        if (ConnectSocket == INVALID_SOCKET)
        {
            printf("Unable to connect to server!\n");
            WSACleanup();
            return false;
        }
 
        return true;
    };
 
    // Free the resouces
    void Stop() {
        int iResult = shutdown(ConnectSocket, SD_SEND);
 
        if (iResult == SOCKET_ERROR)
        {
            printf("shutdown failed: %d\n", WSAGetLastError());
        }
 
        closesocket(ConnectSocket);
        WSACleanup();
    };
 
    // Send message to server
    bool Send(char* szMsg)
    {
        
        int iResult = send(ConnectSocket, szMsg, strlen(szMsg), 0);
 
        if (iResult == SOCKET_ERROR)
        {
            printf("send failed: %d\n", WSAGetLastError());
            Stop();
            return false;
        }
 
        return true;
    };
 
    // Receive message from server
    char* Recv()
    {
        char recvbuf[DEFAULT_BUFFER_LENGTH];
        int iResult = recv(ConnectSocket, recvbuf, DEFAULT_BUFFER_LENGTH, 0);
 
        if (iResult > 0)
        {
            char msg[DEFAULT_BUFFER_LENGTH];
            memset(&msg, 0, sizeof(msg));
            strncpy(msg, recvbuf, iResult);
 
           
 
            return msg;
        }
 
 
        return "ERROR";
    }
 
private:
    char* szServerName;
    SOCKET ConnectSocket;
};
 

//
////This function demonstrates: How to call a callback that is in a PAWN script.
//// CallBacks
//static cell KeyDownGP(cell playerid, cell key, AMX *amx)
//{
//  int idx;
//   //Pawn callback: forward OnPawnCallbackEmitted(playerid,reason);
//    if(!amx_FindPublic(amx, "OnPlayerKeyDownGP", &idx))
//    {
//        cell
//            ret;
//            
//
//        //Here we push our arguments to our function. Note that if the function has multiple arguments you have to push your
//        //values in reverse order! Thats why we're pushing the string first, then the array, and finally our integer.
//
//        amx_Push(amx, key);
//        //amx_PushArray(amx, NULL, NULL, arr, sizeof(arr) / sizeof(cell));
//
//       
//        
//        //Push our integer value
//        amx_Push(amx, playerid);
//
//        //Execute our function using our previously obtained idx var.
//        //Note: This function's second parameter is what the callback returned (Can be NULL if you're not interested in return values).
//        amx_Exec(amx, &ret, idx);
//
//
//    }
//    return 1;
//}
//
//
//static cell SendData(cell playerid, char* pdata, AMX *amx)
//{
//     int idx;
//   //Pawn callback: forward OnPawnCallbackEmitted(playerid,reason);
//    if(!amx_FindPublic(amx, "OnGIGDataSend", &idx))
//    {
//        cell
//            ret,	   addr;
//
//        //Here we push our arguments to our function. Note that if the function has multiple arguments you have to push your
//        //values in reverse order! Thats why we're pushing the string first, then the array, and finally our integer.
//
//       //  amx_Push(amx, key);
//        //amx_PushArray(amx, NULL, NULL, arr, sizeof(arr) / sizeof(cell));
//	
//
//        //Here we push our arguments to our function. Note that if the function has multiple arguments you have to push your
//        //values in reverse order! Thats why we're pushing the string first, then the array, and finally our integer.
//
//        amx_PushString(amx, &addr, NULL, pdata, NULL, NULL);
//       
//        
//        //Push our integer value
//		amx_Push(amx, playerid);
//
//        //Execute our function using our previously obtained idx var.
//        //Note: This function's second parameter is what the callback returned (Can be NULL if you're not interested in return values).
//        amx_Exec(amx, &ret, idx);
//		
//		amx_Release(amx, addr);
//
//
//    }
//    return 1;
//}
//


static char* SendData(char* data)
{
	char* received = new char[50];
 
    Client client("127.0.0.1");

    if (!client.Start())
	{
		return "ERROR";
	}
		logprintf("Send %s", data);
        client.Send(data);
		received = client.Recv();

    client.Stop();
	return received;
}
static cell ReturnFloat(float numb)
{
 return amx_ftoc(numb);
}
static char* GetString(AMX *amx, cell params)
{
 int 
        len = 0,
        ret = 0;

    cell *addr;

    //Get the address of our string param (str) and then get its length
    amx_GetAddr(amx, params, &addr); 
    amx_StrLen(addr, &len);

    //if the length of input isnt 0
    if(len)
    {
        //We increase len because we want to make room for the terminating null char when we allocate memory.
        //Also because GetString's size parameter counts the null chracter, we have to specify the length
        //of the string + 1; otherwise our string will be truncated to make room for the null char (we'd lose 1 character).
        len++;

        //Allocate memory to hold our string we're passing (str) and then "get" the string using our allocated memory to store it.
        char* text = new char[ len ];
        amx_GetString(text, addr, 0, len);
        return text;

    }
	else
		return "";
}

std::string SubstringOfCString(const char *cstr,    size_t start, size_t length)
{
    assert(start + length <= strlen(cstr));
    return std::string(cstr + start, length);
}


// MEMBERS
static cell AMX_NATIVE_CALL PlayerLoginGP(AMX *amx, cell *params)
{  
	char* username = GetString(amx, params[1]) ;
	int playerid = params[2];

	logprintf("Logged In :  %s   %d ", username, playerid );      
	char* g = new char[70];
	 sprintf (g, "%d<=>%s<=>LOGIN", playerid, username );

	 if(SendData(g) == "OK")
		 return 1;
	 else
		 return 0;

}
static cell AMX_NATIVE_CALL PlayerLogoutGP(AMX *amx, cell *params)
{  
	int playerid = params[1];

	logprintf("Logged Out :   %d ", playerid );      
	char* g = new char[16];

  sprintf (g, "%d<=>LOGOUT", playerid);

	
	 if(SendData(g) == "OK")
		 return 1;
	 else
		 return 0;
}
static cell AMX_NATIVE_CALL SayTTS(AMX *amx, cell *params)
{  
	char* g = new char[25];
	 int x = params[1];
	 	 int v = params[2];
  sprintf (g, "%d<=>%d<=>SAY_TTS", x,v);
	 if(SendData(g) == "OK")
		 return 1;
	 else
		 return 0;
}
static cell AMX_NATIVE_CALL StopTTS(AMX *amx, cell *params)
{  
	char* g = new char[25];
	 int x = params[1];
  sprintf (g, "%d<=>STOP_TTS", x);
	 if(SendData(g) == "OK")
		 return 1;
	 else
		 return 0;
}
static cell AMX_NATIVE_CALL ReadUserKey(AMX *amx, cell *params)
{  
	char* g = new char[25];
	 int x = params[1];
	 int keyp = 0;
  sprintf (g, "%d<=>READ_KEY", x);
  char* re = SendData(g);

  keyp = atoi(re);			
			 return keyp;
	  
}
static cell AMX_NATIVE_CALL ReadUserPrefKey(AMX *amx, cell *params)
{  
	char* g = new char[25];
	 int x = params[1];
	 int mod = params[2];
	 int keyp = 0;
  sprintf (g, "%d<=>%d<=>READ_KEYP",x ,mod);
  char* re = SendData(g);

  keyp = atoi(re);			
			 return keyp;
	  
}
static cell AMX_NATIVE_CALL StartStreaming(AMX *amx, cell *params)
{  
	char* g = new char[25];
	 int x = params[1];

  sprintf (g, "%d<=>STREAM_B", x);

  if(SendData(g) == "OK")		
			 return 1;
  else return 0;
	  
}
static cell AMX_NATIVE_CALL PlayStream(AMX *amx, cell *params)
{  
	char* g = new char[25];
	 int x = params[1];
	 	 int vid = params[2];

  sprintf (g, "%d<=>%d<=>STREAM_P", x,vid);

   if(SendData(g) == "OK")		
			 return 1;
  else return 0;
	  
}

PLUGIN_EXPORT bool PLUGIN_CALL Load(void **ppData)
{
   pAMXFunctions = ppData[PLUGIN_DATA_AMX_EXPORTS];
   logprintf = (logprintf_t)ppData[PLUGIN_DATA_LOGPRINTF];
   return 1;
}

PLUGIN_EXPORT void PLUGIN_CALL Unload()
{
}

AMX_NATIVE_INFO projectNatives[] =
{
		   { "ReadUserKey", ReadUserKey },	
			 { "ReadUserPrefKey", ReadUserPrefKey },	
			  { "StartStreaming", StartStreaming },	
			   { "PlayStream", PlayStream },	
	   { "PlayerLoginGP", PlayerLoginGP },	
	   { "PlayerLogoutGP", PlayerLogoutGP } ,
		  { "SayTTS", SayTTS } ,
		  	  { "StopTTS", StopTTS } ,
	   {0, 0}// In the first array dimension, you write the name of the native you're going to call in PAWN. In the second one, you write the name in .cpp file. In this case, they're the same!
};

PLUGIN_EXPORT unsigned int PLUGIN_CALL Supports()
{
   return SUPPORTS_VERSION | SUPPORTS_AMX_NATIVES;
}

PLUGIN_EXPORT int PLUGIN_CALL AmxLoad(AMX *amx)
{
   return amx_Register(amx, projectNatives, -1);
}

PLUGIN_EXPORT int PLUGIN_CALL AmxUnload(AMX *amx)
{
   return AMX_ERR_NONE;
}