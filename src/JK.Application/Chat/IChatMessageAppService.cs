using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services;

namespace JK.Chat
{
    public interface IChatAppService : IApplicationService
    {
        void SendMessage(long chatGroupId, long userId, string message);

        void CreatePrivate();

        void CreateGroup();

        void JoinGroup();

        void LeaveGroup();
    }

    public class ChatAppService : IChatAppService
    {
        public void CreateGroup()
        {
            throw new NotImplementedException();
        }

        public void CreatePrivate()
        {
            throw new NotImplementedException();
        }

        public void CreatePrivateChat()
        {
            throw new NotImplementedException();
        }

        public void JoinGroup()
        {
            throw new NotImplementedException();
        }

        public void LeaveGroup()
        {
            throw new NotImplementedException();
        }

        public void SendMessage(long chatGroupId, long userId, string message)
        {
            throw new NotImplementedException();
        }
    }
}
