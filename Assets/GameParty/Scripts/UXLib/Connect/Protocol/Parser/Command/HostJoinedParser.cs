using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Parser.Command
{
    class HostJoinedParser : UXProtocolParser
    {
        public override JSONNode Parse(byte[] array)
        {
            JSONNode data = base.baseParse(UXProtocol.Command_Parse.host_joined, array);
           

            return data;
        }
    }
}