using Application.Service.Chats;
using Application.Service.Chats.ChatGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.FacadPatterns
{
    public interface IChatsFacad
    {
        IChatService ChatService { get; }
        IChatGrpupsService ChatGrpupsService { get; }
    }
}
