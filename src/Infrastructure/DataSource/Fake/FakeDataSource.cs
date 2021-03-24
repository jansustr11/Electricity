using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KMB.DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Utils;
using Electricity.Infrastructure.DataSource.Fake;

namespace Electricity.Infrastructure.DataSource
{
    public class FakeDataSource : KMB.DataSource.DataSource
    {
        private int _seed;

        private BoundedInterval _interval { get; set; }

        public List<Group> Groups { get; set; }

        public FakeDataSource(int seed, BoundedInterval interval)
        {
            _seed = seed;
            _interval = interval;
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override List<Group> GetGroups(Guid parent)
        {
            throw new NotImplementedException();
        }

        public override Quantity[] GetQuantities(Guid groupID, byte arch, DateRange range)
        {
            throw new NotImplementedException();
        }

        public override RowCollection GetRows(Guid groupID, byte arch, DateRange range, Quantity[] quantities, uint aggregation, EEnergyAggType energyAggType = EEnergyAggType.Cumulative)
        {
            if (Groups.FindIndex(g => g.ID == groupID) == -1)
            {
                throw new ArgumentException("invalid groupId");
            }

            bool cumulative = arch == (byte)Arch.ElectricityMeter;

            var generators = quantities.Select((quant) =>
            {
                int groupIdHash = GuidToHash(groupID);
                int rangeHash = range != null ? DateRangeToHash(range) : 0;
                int quanityHash = QuantityToHash(quant);
                var hashSeed = groupIdHash + rangeHash + quanityHash;
                var g = new RandomSeries(0, _seed + hashSeed);
                g.Cumulative = cumulative;
                return g;
            }).ToArray();

            foreach (var q in quantities)
            {
                q.Value = new FakePropValueFloat();
            }

            var interval = _interval.ToInterval();

            if (range != null)
            {
                interval = interval.GetOverlap(Interval.FromDateRange(range));

                if (interval == null)
                {
                    return new FakeRowCollection();
                }
            }

            IEnumerable<RowInfo> GenerateRows()
            {
                DateTime time = interval.Start ?? _interval.Start;
                TimeSpan duration = new TimeSpan(0, 0, 10);
                while (time < interval.End)
                {
                    var rowValues = generators.Select(g => g.Next()).ToArray();
                    for (int i = 0; i < generators.Length; i++)
                    {
                        var propValue = quantities[i].Value as FakePropValueFloat;
                        propValue.Value = generators[i].Next();
                    }
                    yield return new RowInfo(time, 0, null);
                    time += duration;
                }
            }

            return new FakeRowCollection(GenerateRows());
        }

        public override List<Group> GetUserGroups(Guid user)
        {
            return Groups;
        }

        private int GuidToHash(Guid guid)
        {
            return StringToHash(guid.ToString());
        }

        private int DateRangeToHash(DateRange range)
        {
            var min = range.DateMin.ToString();
            var max = range.DateMax.ToString();
            return StringToHash(min) + StringToHash(max);
        }

        private int QuantityToHash(Quantity quant)
        {
            return StringToHash(quant.PropName);
        }

        private int StringToHash(string str)
        {
            int value = 0;
            var chars = str.ToCharArray();

            foreach (var c in chars)
            {
                value += c;
            }

            return value;
        }

        public override IDisposable NewConnection()
        {
            throw new NotImplementedException();
        }

        public override IDisposable BeginTransaction(IDisposable connection)
        {
            throw new NotImplementedException();
        }

        public override void CommitTransaction(IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override void RollbackTransaction(IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override Guid Login(string ENVISUser, string ENVISPassword, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override List<Group> GetUserGroups(Guid user, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override List<Group> GetGroups(Guid parent, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override Quantity[] GetQuantities(Guid GroupID, byte arch, DateRange range, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override RowCollection GetRows(Guid GroupID, byte arch, DateRange range, Quantity[] quantities, uint aggregation, IDisposable connection, IDisposable transaction, EEnergyAggType energyAggType = KMB.DataSource.EEnergyAggType.Cumulative)
        {
            throw new NotImplementedException();
        }

        protected override IList<UniConfig> GetConfs(int RecID, DateRange range, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        protected override IList<UniArchiveBinPack> GetBinPacks(int RecID, byte arch, DateRange range, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        protected override Stream GetStreamToRead(int RecID, byte arch, DateTime keyTime, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        protected override UniArchiveBinPack BinPackToWrite(int recID, byte archID, DateTime time, ref DateTime curIntStart, ref DateTime curIntEnd, UniArchiveDefinition uad, uint period, IDisposable connection, IDisposable transaction, bool directFS)
        {
            throw new NotImplementedException();
        }

        protected override void CommitBinPack(int recID, byte archID, UniArchiveBinPack ua, DateTime time, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override int SaveRecord(SmpMeasNameDB record, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override void SaveArchiveDefinition(UniArchiveDefinition uad, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override void SaveConfig(UniConfig cfg, int recID, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override List<SmpMeasNameDB> GetRecords(IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }

        public override void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public override GroupInfo GetGroupInfos(Guid ID, InfoFilter filter, IDisposable connection, IDisposable transaction)
        {
            throw new NotImplementedException();
        }
    }
}