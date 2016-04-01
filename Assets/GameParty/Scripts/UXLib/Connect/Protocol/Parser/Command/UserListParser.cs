using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Parser.Command
{
    class UserListParser : UXProtocolParser
    {
        public override JSONNode Parse(byte[] array)
        {
            JSONNode data = base.baseParse(UXProtocol.Command_Parse.user_list, array);
            //length, u_code,list_len, name
            

            return data;
        }
    }
}
