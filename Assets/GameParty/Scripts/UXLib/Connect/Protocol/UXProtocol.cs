using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UXLib.Connect.Protocol.Generator;
using UXLib.Connect.Protocol.Parser;

namespace UXLib.Connect.Protocol
{
    class UXProtocol
    {
        public enum Command_Generate
        {
            join = 0, // 앞으로 여기다가 커맨드 추가
            update_user_index = 1,
            update_ready_count = 2,
            change_lobby_state = 3,
            broadcast = 4,
            send_target = 5,
            send_host = 6,
            get_user_list = 7,
            start_game = 8,
            restart_game = 9,
            result_game = 10,
            end_game = 11,
            exit = 12,
            max_user_set = 13,
            premium_user = 14
        }

        public enum Command_Parse
        {
            ack_result = 0
            ,user_add = 1
            ,user_list = 2
            ,user_del = 3
            ,update_user_index_result = 4
            ,send_error = 5
            ,exit_result = 6
            ,host_close = 7
            ,data = 8
            ,check_network_state_result = 9
            ,start_game_result = 10
            ,restart_game_result = 11
            ,result_game_result = 12
            ,end_game_result = 13
            ,host_joined = 14
            ,get_user_list_result = 15
            ,update_ready_count_result = 16
            ,join_result = 17
            ,change_lobby_state_result = 18
            ,report_network_state_result = 19
            ,premium_user_result = 20
        }

        private static UXProtocol instance = null;
        public static UXProtocol Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UXProtocol();
                }
                return instance;
            }
        }

        public UXProtocol()
        {
        }

        public UXProtocolGenerator GeneratorFactory(Command_Generate command)
        {
            return UXProtocolGenerator.Factory(command);
        }

        public UXProtocolGenerator GeneratorFactory(string command)
        {
            return UXProtocolGenerator.Factory((Command_Generate)Enum.Parse(typeof(Command_Generate), command, true));
        }

        public UXProtocolParser ParserFactory(Command_Parse command)
        {
            return UXProtocolParser.Factory(command);
        }

        public UXProtocolParser ParserFactory(int command)
        {
            return UXProtocolParser.Factory((Command_Parse)command);
        }
    }
}
