using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Generator.Command
{
    class SendDataToCodeGenerator : UXProtocolGenerator
    {
        public override byte[] Generate(JSONNode data)
        {
            base.baseGenerate(UXProtocol.Command_Generate.send_target, data);
            //length, target, data
            string _data = data["data"];
            var target = data["target"];

            int length = 1+_data.Length +(4* target.Count);
            AddByte8((byte)length);

            AddByte8((byte)target.Count);//target_len
            for (int i = 0, len = target.Count; i < len; i++)
            {
                AddByte32(target[i].AsInt);
            }
            AddByteString(_data);

            return byteList.ToArray();
        }
    }
}
