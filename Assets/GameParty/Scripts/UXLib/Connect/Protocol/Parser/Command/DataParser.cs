using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

using UnityEngine;

namespace UXLib.Connect.Protocol.Parser.Command
{
    class DataParser : UXProtocolParser{
        public override JSONNode Parse(byte[] array)
        {
            JSONNode data = base.baseParse(UXProtocol.Command_Parse.data, array);

            int sender = BitConverter.ToInt32(array, 2);
            data["sender"].AsInt = sender;

            int dataLen = array[6];

            Debug.Log(dataLen);

            string msg = new UTF8Encoding().GetString(array, 7, dataLen);

            data["data"] = msg;
            
            return data;
        }
    }
}