using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace UXLib.Connect.Protocol.Generator.Command
{
    class SetPlayerStateGenerator : UXProtocolGenerator
    {
        public override byte[] Generate(JSONNode data)
        {
            base.baseGenerate(UXProtocol.Command_Generate.change_lobby_state, data);
            //length, state (wait:0, ready:1)
            AddByte8(1);//length
            string state = data["state"];
            if(state=="wait") AddByte8((byte)0);
            else AddByte8((byte)1);

            return byteList.ToArray();
        }
    }
}



