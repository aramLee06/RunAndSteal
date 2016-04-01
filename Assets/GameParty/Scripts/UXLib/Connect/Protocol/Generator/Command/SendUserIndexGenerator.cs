using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
namespace UXLib.Connect.Protocol.Generator.Command
{
    class SendUserIndexGenerator : UXProtocolGenerator
    {
        public override byte[] Generate(JSONNode data)
        {
            base.baseGenerate(UXProtocol.Command_Generate.update_user_index, data);
            //length, index
            AddByte8(1);//length
            AddByte8((byte)data["index"].AsInt);

            return byteList.ToArray();
        }
    }
}




