using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Parser.Command
{
    class UpdateReadyCountResultParser : UXProtocolParser
    {
        public override JSONNode Parse(byte[] array)
        {
            JSONNode data = base.baseParse(UXProtocol.Command_Parse.update_ready_count_result, array);

            int ready = array[2];
            int total = array[3];

            data["ready"].AsInt = ready;
            data["total"].AsInt = total;

            return data;
        }
    }
}