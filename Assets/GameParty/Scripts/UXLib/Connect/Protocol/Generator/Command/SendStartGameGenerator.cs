using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Generator.Command
{
    class SendStartGameGenerator : UXProtocolGenerator
    {
        public override byte[] Generate(JSONNode data)
        {
            base.baseGenerate(UXProtocol.Command_Generate.start_game, data);
            AddByte8(0);//length              

            return byteList.ToArray();
        }
    }
}
