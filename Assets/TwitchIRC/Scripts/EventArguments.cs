using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irc
{
    public class UpdateUsersEventArgs : EventArgs
    {
        public string Channel { get; internal set; }
        public string[] UserList { get; internal set; }
        public UpdateUsersEventArgs(string Channel, string[] UserList)
        {
            this.Channel = Channel;
            this.UserList = UserList;
        }
    }
    public class UserJoinedEventArgs : EventArgs
    {
        public string Channel { get; internal set; }
        public string User { get; internal set; }
        public UserJoinedEventArgs(string Channel, string User)
        {
            this.Channel = Channel;
            this.User = User;
        }
    }
    public class UserLeftEventArgs : EventArgs
    {
        public string Channel { get; internal set; }
        public string User { get; internal set; }
        public UserLeftEventArgs(string Channel, string User)
        {
            this.Channel = Channel;
            this.User = User;
        }
    }

    public class ChannelMessageEventArgs : EventArgs
    {
        public string Channel { get; internal set; }
        public string From { get; internal set; }
        public string Message { get; internal set; }
        public ChannelMessageEventArgs(string Channel, string From, string Message)
        {
            this.Channel = Channel;
            this.From = From;
            this.Message = Message;
        }
    }

    public class StringEventArgs : EventArgs
    {
        public string Result { get; internal set; }
        public StringEventArgs(string s) { Result = s; }
        public override string ToString() { return Result; }
    }
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; internal set; }
        public ExceptionEventArgs(Exception x) { Exception = x; }
        public override string ToString() { return Exception.ToString(); }
    }
}
