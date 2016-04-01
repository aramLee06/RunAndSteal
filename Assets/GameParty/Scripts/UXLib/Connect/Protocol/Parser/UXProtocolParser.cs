using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SimpleJSON;

using UXLib.Connect.Protocol.Parser.Command;

namespace UXLib.Connect.Protocol.Parser
{
    abstract class UXProtocolParser
    {

        public static UXProtocolParser Factory(UXProtocol.Command_Parse command)
        {
            UXProtocolParser parser;
            switch (command)
            {
                case UXProtocol.Command_Parse.ack_result:
                    parser = new AckResultParser();
                    break;
                case UXProtocol.Command_Parse.user_add:
                    parser = new UserAddParser();
                    break;
                case UXProtocol.Command_Parse.user_del:
                    parser = new UserDelParser();
                    break;
                case UXProtocol.Command_Parse.update_user_index_result:
                    parser = new UpdateUserIndexResultParser();
                    break;
                case UXProtocol.Command_Parse.send_error:
                    parser = new SendErrorParser();
                    break;
                case UXProtocol.Command_Parse.exit_result:
                    parser = new ExitResultParser();
                    break;
                case UXProtocol.Command_Parse.host_close:
                    parser = new HostCloseParser();
                    break;
                case UXProtocol.Command_Parse.data:
                    parser = new DataParser();
                    break;
                case UXProtocol.Command_Parse.check_network_state_result:
                    parser = null;
                    break;
                case UXProtocol.Command_Parse.start_game_result:
                    parser = new StartGameResultParser();
                    break;
                case UXProtocol.Command_Parse.restart_game_result:
                    parser = new RestartGameResultParser();
                    break;
                case UXProtocol.Command_Parse.result_game_result:
                    parser = new ResultGameResultParser();
                    break;
                case UXProtocol.Command_Parse.end_game_result:
                    parser = new EndGameResultParser();
                    break;
                case UXProtocol.Command_Parse.host_joined:
                    parser = new HostJoinedParser();
                    break;
                case UXProtocol.Command_Parse.get_user_list_result:
                    parser = new GetUserListResultParser();
                    break;
                case UXProtocol.Command_Parse.update_ready_count_result:
                    parser = new UpdateReadyCountResultParser();
                    break;
                case UXProtocol.Command_Parse.join_result:
                    parser = new JoinResultParser();
                    break;
                case UXProtocol.Command_Parse.change_lobby_state_result:
                    parser = new ChangeLobbyStateResultParser();
                    break;
                case UXProtocol.Command_Parse.report_network_state_result:
                    parser = null;
                    break;
                case UXProtocol.Command_Parse.premium_user_result:
                    parser = new PremiumUserResultParser();
                    break;
                default :
                    parser = null;
                    break;
            }

            return parser;
        }

   

        protected JSONNode baseParse(UXProtocol.Command_Parse command, byte[] array)
        {
            JSONClass data = new JSONClass(); // byte array 분석하여 추출한 데이터

            data["cmd"] = command.ToString();

            return data;
        }

        public abstract JSONNode Parse(byte[] array);
    }
}
