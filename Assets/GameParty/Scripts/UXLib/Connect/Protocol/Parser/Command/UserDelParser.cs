using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Parser.Command
{
    class UserDelParser : UXProtocolParser
    {
        public override JSONNode Parse(byte[] array)
        {
            JSONNode data = base.baseParse(UXProtocol.Command_Parse.user_del, array);
            //u_code,list_len,list

            data["u_code"].AsInt = BitConverter.ToInt32(array, 2);
            int list_len = array[6];

            for (int i = 0; i < list_len; i++)
            {
                int code = BitConverter.ToInt32(array, 7 + (i * 4));
                data["user_list"][i] = code + "." + "Player " + (i + 1);
            }

        
            return data;
        }
    }
}
