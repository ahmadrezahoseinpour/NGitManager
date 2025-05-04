using System;
using System.Collections.Generic;
using System.Text;

namespace GitManager.Dto.Issue
{
    /// <summary>
    /// Specifies an assignee ID for a data query. It can be specified via
    /// - an integer (the actual assignee ID on GitLab):    queries items assigned to the given user
    /// - 'Any':                                            queries all assigned items
    /// - 'None':                                           queries all unassigned items
    /// </summary>
    public sealed class QueryAssigneeIdDto
    {
        private readonly string _id;

        private QueryAssigneeIdDto(string id)
        {
            _id = id;
        }

        public static QueryAssigneeIdDto Any { get; } = new QueryAssigneeIdDto("Any");

        public static QueryAssigneeIdDto None { get; } = new QueryAssigneeIdDto("None");

        public static implicit operator QueryAssigneeIdDto(long id)
        {
            if (id == 0)
                return None;

            return new QueryAssigneeIdDto(id.ToString());
        }

        public override string ToString()
        {
            return _id;
        }
    }
}
