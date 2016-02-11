﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Parser.Command
{
    class UserAddParser : UXProtocolParser
    {
        public override JSONNode Parse(byte[] array)
        {
            JSONNode data = base.baseParse(UXProtocol.Command_Parse.user_add, array);
            //u_code,list_len, name

            data["u_code"].AsInt = BitConverter.ToInt16(array, 2);
            int list_len = array[4];

            for (int i = 0; i < list_len; i++)
            {
                int code = BitConverter.ToInt16(array, 5 + (i * 2));
                data["user_list"][i] = code + "." + "Player " + (i + 1);
                if (data["u_code"].AsInt == code)
                {
                    data["name"] = data["user_list"][i];
                }
            }

            return data;
        }
    }
}
