using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Series;
using DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Services;
using MediatR;

namespace Electricity.Application.Series.Queries.GetQuantitySeries
{
    public class GetQuantitySeriesQuery : IRequest<ITimeSeries<float>>, IGetQuantitySeriesQuery
    {
        public Guid GroupId { get; set; }

        public byte Arch { get; set; }

        public Quantity Quantity { get; set; }

        public Tuple<DateTime, DateTime> Range { get; set; }

        public AggregationMethod AggregationMethod { get; set; }

        public TimeSpan AggregationInterval { get; set; }
    }

    public class GetQuantitySeriesQueryHandler : IRequestHandler<GetQuantitySeriesQuery, ITimeSeries<float>>
    {
        private readonly QuantitySeriesService _service;
        private readonly IMapper _mapper;

        public GetQuantitySeriesQueryHandler(QuantitySeriesService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public Task<ITimeSeries<float>> Handle(GetQuantitySeriesQuery request, CancellationToken cancellationToken)
        {
            var series = _service.GetSeries(request);

            return Task.FromResult(series);
        }
    }

}