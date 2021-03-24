using System;
using System.Collections.Generic;
using KMB.DataSource;
using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Queries;



namespace Electricity.Infrastructure.DataSource
{
    public class DataSourceTableReader : ITable
    {
        private KMB.DataSource.DataSource _source;
        private Guid _groupId;
        private byte _arch;

        public DataSourceTableReader(KMB.DataSource.DataSource source, Guid groupId, byte arch)
        {
            _source = source;
            _groupId = groupId;
            _arch = arch;
        }

        unsafe public Interval GetInterval()
        {
            DateTime? start = null;
            DateTime end = new DateTime();

            var quants = new Quantity[]
            {
                new Quantity("", "")
            };

            using (var rc = _source.GetRows(_groupId, _arch, null, quants, 0))
            {
                if (rc == null)
                {
                    return null;
                }

                fixed (byte* p = rc.Buffer)
                {
                    rc.SetPointer(p);
                    foreach (KMB.DataSource.RowInfo row in rc)
                    {
                        if (start == null)
                            start = row.TimeLocal;
                        else
                            end = row.TimeLocal;
                    }
                }
            }

            return new Interval(start, end);
        }

        unsafe public IEnumerable<Tuple<DateTime, float[]>> GetRows(GetRowsQuery query)
        {
            if (query.Interval == null)
            {
                throw new ArgumentNullException(nameof(query.Interval));
            }

            var quants = query.Quantities;
            int rowLen = quants.Length;

            var entries = new List<Tuple<DateTime, float[]>>();
            var dateRange = query.Interval.ToDateRange();
            float[] arr;
            using (var rc = _source.GetRows(_groupId, _arch, dateRange, quants, query.Aggregation, query.EnergyAggType))
            {
                if (rc == null)
                {
                    return entries;
                }

                fixed (byte* p = rc.Buffer)
                {
                    rc.SetPointer(p);
                    foreach (KMB.DataSource.RowInfo row in rc)
                    {
                        arr = new float[rowLen];
                        for (int i = 0; i < rowLen; i++)
                        {
                            arr[i] = (quants[i].Value.GetValue() as float?) ?? float.NaN;
                        }
                        entries.Add(Tuple.Create(row.TimeLocal, arr));
                    }
                }
            }

            if (query.Interval.IsHalfBounded)
            {
                return Slice(entries, query.Interval);
            }

            return entries;
        }

        public IEnumerable<Tuple<DateTime, float[]>> Slice(IEnumerable<Tuple<DateTime, float[]>> rows, Interval interval)
        {
            throw new NotImplementedException();
        }
    }
}