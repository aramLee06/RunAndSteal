using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Generator.Command
{
    class SendDataToHostGenerator : UXProtocolGenerator
    {
         public override byte[] Generate(JSONNode data)
        {
            base.baseGenerate(UXProtocol.Command_Generate.send_host, data);
            //length,  data
            string _data = data["data"];
            int length = _data.Length;
            AddByte8((byte)length);
            AddByteString(_data);

            return byteList.ToArray();
        }
    }
}
