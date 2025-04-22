using System;
using System.Collections.Generic;
using System.Text;

namespace GitManager.Dto.Issue
{
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
