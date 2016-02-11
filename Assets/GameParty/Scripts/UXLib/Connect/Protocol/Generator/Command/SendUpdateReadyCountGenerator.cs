using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Generator.Command
{
    class SendUpdateReadyCountGenerator : UXProtocolGenerator
    {
        public override byte[] Generate(JSONNode data)
        {
            base.baseGenerate(UXProtocol.Command_Generate.update_ready_count, data);
            //length, ready, total 
            AddByte8(2);//length,
            AddByte8((byte)data["ready"].AsInt);
            AddByte8((byte)data["total"].AsInt);           

            return byteList.ToArray();
        }
    }
}
