using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SimpleJSON;

using UnityEngine;

using UXLib.Connect.Protocol.Generator.Command;

namespace UXLib.Connect.Protocol.Generator
{
    abstract class UXProtocolGenerator
    {
        protected List<byte> byteList;

        public static UXProtocolGenerator Factory(UXProtocol.Command_Generate command)
        {
            UXProtocolGenerator parser;
            switch (command)
            {
                case UXProtocol.Command_Generate.join:
                    parser = new JoinGenerator();
                    break;
                case UXProtocol.Command_Generate.update_user_index:
                    parser = new SendUserIndexGenerator();
                    break;
                case UXProtocol.Command_Generate.update_ready_count:
                    parser = new SendUpdateReadyCountGenerator();
                    break;
                case UXProtocol.Command_Generate.change_lobby_state:
                    parser = new SetPlayerStateGenerator();
                    break;
                case UXProtocol.Command_Generate.broadcast:
                    parser = new SendDataGenerator();
                    break;
                case UXProtocol.Command_Generate.send_target:
                    parser = new SendDataToCodeGenerator();                    
                    break;
                case UXProtocol.Command_Generate.send_host:
                    parser = new SendDataToHostGenerator();
                    break;
                case UXProtocol.Command_Generate.get_user_list:
                    parser = new RefreshUserListFromServerGenerator();
                    break;
                case UXProtocol.Command_Generate.start_game:
                    parser = new SendStartGameGenerator();
                    break;
                case UXProtocol.Command_Generate.restart_game:
                    parser = new SendRestartGameGenerator();
                    break;
                case UXProtocol.Command_Generate.result_game:
                    parser = new SendResultGameGenerator();
                    break;
                case UXProtocol.Command_Generate.end_game:
                    parser = new SendEndGameGenerator();
                    break;
                case UXProtocol.Command_Generate.exit:
                    parser = new SendExitGenerator();
                    break;
                case UXProtocol.Command_Generate.max_user_set:
                    parser = new SendMaxUserGenerator();
                    break;
                case UXProtocol.Command_Generate.premium_user:
                    parser = new PremiumUserGenerator();
                    break;
                default:
                    parser = null;
                    break;
            }
                
            return parser;
        }

        public UXProtocolGenerator() {
            byteList = new List<byte>();
        }

        protected void baseGenerate(UXProtocol.Command_Generate command, JSONNode data)
        {
            // 여기서 cmd, l_code, u_code 추가 
            byteList.Add((byte)command);
            AddByte16(data["l_code"].AsInt == 0 ? -1 : data["l_code"].AsInt);
            AddByte32(data["u_code"].AsInt == 0 ? -1 : data["u_code"].AsInt);    

        }

        protected void AddByte8(byte value)
        {
            byteList.Add(value);
        }

        protected void AddByte16(int val)
        {
            Int16 value = (Int16)val;
            byte[] bytes = BitConverter.GetBytes(value);
            foreach (byte v in bytes)
            {
                byteList.Add(v);
            }
        }

		protected void AddByte32(int val)
		{
			byte[] bytes = BitConverter.GetBytes(val);
			foreach (byte v in bytes)
			{
				byteList.Add(v);
			}
		}

        protected void AddByteString(string str)
        {
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			for (int i = 0; i < bytes.Length; i++) 
			{
				byte v = bytes [i];
				byteList.Add(v);
			}
        }

        public abstract byte[] Generate(JSONNode data);
        
    }
}
