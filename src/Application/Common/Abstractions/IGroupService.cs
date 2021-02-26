using DataSource;
using Electricity.Application.Common.Models;
using System;

namespace Electricity.Application.Common.Interfaces
{
    public interface IGroupService
    {
        public Group[] GetUserGroups();

        public GroupTreeNode GetUserGroupTree();
    }
}