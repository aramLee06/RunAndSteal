using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Parser.Command
{
    class GetUserListResultParser : UXProtocolParser
    {
        public override JSONNode Parse(byte[] array)
        {
            JSONNode data = base.baseParse(UXProtocol.Command_Parse.get_user_list_result, array);
            //length, list_len, user list[]
            int list_len = array[2];

			data ["user_list"] = new JSONArray ();
            for (int i = 0; i < list_len; i++)
            {
                int code = BitConverter.ToInt32(array, 3 + (i * 4));
                data["user_list"][i] = code + "." + "Player " + (i + 1);
            }


            return data;
        }
    }
}