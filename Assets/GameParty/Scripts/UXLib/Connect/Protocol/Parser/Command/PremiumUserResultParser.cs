using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Parser.Command
{
    class PremiumUserResultParser : UXProtocolParser
    {
        public override JSONNode Parse(byte[] array)
        {
            JSONNode data = base.baseParse(UXProtocol.Command_Parse.premium_user_result, array);
            //u_code
            data["u_code"].AsInt = BitConverter.ToInt16(array, 2);
            return data;
        }
    }
}
