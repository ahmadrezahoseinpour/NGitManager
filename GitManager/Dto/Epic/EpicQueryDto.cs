using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitManager.Dto.Epic
{
    public class EpicQueryDto
    {
        public EpicState? State { get; set; }

        public string OrderBy { get; set; }

        public string Sort { get; set; }

        public string Labels { get; set; }

        public DateTime? CreatedAfter { get; set; }

        public DateTime? CreatedBefore { get; set; }

        public DateTime? UpdatedAfter { get; set; }

        public DateTime? UpdatedBefore { get; set; }

        public string Search { get; set; }
    }
}
