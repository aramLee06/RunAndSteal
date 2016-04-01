using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Parser.Command
{
    class UpdateUserIndexResultParser : UXProtocolParser
    {
        public override JSONNode Parse(byte[] array)
        {
            JSONNode data = base.baseParse(UXProtocol.Command_Parse.update_user_index_result, array);

            data["index"].AsInt = array[2];

            return data;
        }
    }
}
