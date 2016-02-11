using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Generator.Command
{
    class JoinGenerator : UXProtocolGenerator
    {
        public override byte[] Generate(JSONNode data)
        {
            base.baseGenerate(UXProtocol.Command_Generate.join, data);
            //length, max_user, package
            string package_name = data["package_name"];
            int length = 1 + package_name.Length;
            AddByte8((byte)length);
            AddByte8((byte)data["max_user"].AsInt);
            AddByteString(data["package_name"]);
            
            return byteList.ToArray();
        }
    }
}
