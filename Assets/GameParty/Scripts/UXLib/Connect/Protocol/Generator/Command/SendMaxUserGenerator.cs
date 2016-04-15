using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Generator.Command
{
    class SendMaxUserGenerator : UXProtocolGenerator
    {
        public override byte[] Generate(JSONNode data)
        {
            base.baseGenerate(UXProtocol.Command_Generate.max_user_set, data);
            //length, max_client         
            AddByte8(1);
            AddByte8((byte)data["max_client"].AsInt);            

            return byteList.ToArray();
        }
    }
}
