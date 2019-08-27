using System;
using System.Collections.Generic;
using System.Text;

namespace GSA_SERVER
{
    public enum GDPCommand : byte
    {
        UNKNOWN = 0,
        LOGIN = 1,
        LOGOUT = 2,
        STATUS = 3,
        READ_KEY = 4,
        KEYDATA = 6,
        PLAY_VOICE = 7,
        LOGGED = 8,
        LOGIN_ERROR = 9,
        ACCESS_TOKEN_ERROR = 10,
        OK = 11,
        ERROR = 12,
        STOP_VOICE = 13,
        RESPONSE = 14,
        READ_PREF_KEY = 15,
        START_STREAM = 16,
        PLAY_STREAM = 17,
        PLAYER_CONNECTED = 18

    }
}
