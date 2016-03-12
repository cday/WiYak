namespace WiYak
{
    public class Commands
    {
        public const string CommandDelimeter = "|";
        public const string Join = "J";
        public const string Leave = "L";
        public const string WhoAmI = "W";
        public const string LobbyMessage = "M";
        public const string Invalid = "I";
        public const string Ready = "R";
        public const string Change = "C";
        public const string PrivateMessage = "PM";
        public const string PrivateLeave = "PL";
        public const string PrivateRequest = "PR";
        public const string PrivateResponse = "PP";
        public const string PrivateFinalize = "PF";

        public const string JoinFormat = Join + CommandDelimeter + "{0}";
        public const string LeaveFormat = Leave + CommandDelimeter + "{0}";
        public const string LobbyMessageFormat = LobbyMessage + CommandDelimeter + "{0}" + CommandDelimeter + "{1}";
        public const string WhoAmIFormat = WhoAmI + CommandDelimeter + "{0}";
        public const string InvalidFormat = Invalid + CommandDelimeter + "{0}";
        public const string ReadyFormat = Ready + CommandDelimeter + "{0}";
        public const string ChangeFormat = Change + CommandDelimeter + "{0}" + CommandDelimeter + "{1}";
        public const string PrivateMessageFormat = PrivateMessage + CommandDelimeter + "{0}" + CommandDelimeter + "{1}" + CommandDelimeter + "{2}";
        public const string PrivateLeaveFormat = PrivateLeave + CommandDelimeter + "{0}" + CommandDelimeter + "{1}";
        public const string PrivateRequestFormat = PrivateRequest + CommandDelimeter + "{0}" + CommandDelimeter + "{1}" + CommandDelimeter + "{2}";
        public const string PrivateResponseFormat = PrivateResponse + CommandDelimeter + "{0}" + CommandDelimeter + "{1}" + CommandDelimeter + "{2}";
        public const string PrivateFinalizeFormat = PrivateFinalize + CommandDelimeter + "{0}" + CommandDelimeter + "{1}";

        public const string True = "t";
        public const string False = "f";
    }
}
