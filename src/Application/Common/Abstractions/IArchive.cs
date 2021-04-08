using System;
using System.Collections.Generic;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Queries;
using KMB.DataSource;

namespace Electricity.Application.Common.Interfaces
{
    public interface IArchive
    {
        Interval GetInterval();
        Quantity[] GetQuantities(DateRange range);

        IEnumerable<Tuple<DateTime, float[]>> GetRows(GetArchiveRowsQuery query);
    }
}