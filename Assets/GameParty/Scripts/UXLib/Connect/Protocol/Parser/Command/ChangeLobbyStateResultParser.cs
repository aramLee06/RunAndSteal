using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Parser.Command
{
    class ChangeLobbyStateResultParser : UXProtocolParser
    {
        public override JSONNode Parse(byte[] array)
        {
            JSONNode data = base.baseParse(UXProtocol.Command_Parse.change_lobby_state_result, array);
            //length, count,, u_code, time
            data["u_code"].AsInt = BitConverter.ToInt32(array, 2);

            int state = array[6];

            if (state == 0)
            {
                data["state"] = "wait";
            }
            else
            {
                data["state"] = "ready";
            }

            return data;
        }
    }
}