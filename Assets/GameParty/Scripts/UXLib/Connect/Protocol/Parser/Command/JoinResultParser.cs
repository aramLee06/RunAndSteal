using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Parser.Command
{
    class JoinResultParser : UXProtocolParser
    {
        public override JSONNode Parse(byte[] array)
        {
            JSONNode data = base.baseParse(UXProtocol.Command_Parse.join_result, array);
            // TODO : cmd가 join일 때만 오는 데이터 Parsing하여 추가
            //length, ack, is_host, list_len, user_list[], user

            data["ack"].AsInt = BitConverter.ToInt16(array, 2);
            data["is_host"].AsBool = array[3] == 0 ? false : true;
            int u_code = BitConverter.ToInt16(array, 5); ;

            int list_len = array[7];

            data["user_list"] = new JSONArray();
            for (int i = 0; i < list_len; i++)
            {
                int code = BitConverter.ToInt16(array, 8 + (i * 2));
                data["user_list"][i] = code + "." + "Player " + (i + 1);
                if (u_code == code)
                {
                    data["user"] = data["user_list"][i];
                }
            }

            if (data["user_list"].AsArray.Count == 0)
            {
                data["user"] = u_code + ".Player 1";
            }

            return data;
        }
    }
}
