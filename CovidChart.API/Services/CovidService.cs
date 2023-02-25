using CovidChart.API.Hubs;
using CovidChart.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CovidChart.API.Services
{
    public class CovidService
    {
        private readonly AppDbContext _con;
        private readonly IHubContext<CovidHub> _hubContext;

        public CovidService(AppDbContext con, IHubContext<CovidHub> hubContext)
        {
            _con = con;
            _hubContext = hubContext;
        }

        public IQueryable<Covid> GetList()
        {
            return _con.Covids.AsQueryable();
        }

        public async Task SaveCovid(Covid covid)
        {
            await _con.Covids.AddAsync(covid);
            await _con.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveCovidList", GetCovidChart());
        }

        public List<Models.CovidChart> GetCovidChart()
        {
            var chart = new List<Models.CovidChart>();

            using (var command = _con.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"select CovidDate, [1], [2], [3], [4], [5] from 
(select [City], [Count], Cast([CovidDate] as date) as CovidDate from Covids) as Covids
 PIVOT(
 SUM([Count]) For City IN ([1], [2], [3], [4], [5])) as pvt order by CovidDate";

                command.CommandType = System.Data.CommandType.Text;

                _con.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Models.CovidChart cc = new Models.CovidChart();

                        cc.CovidDate = reader.GetDateTime(0).ToShortDateString();

                        Enumerable.Range(1, 5).ToList().ForEach(x =>
                        {
                            if (System.DBNull.Value.Equals(reader[x]))
                            {
                                cc.Counts.Add(0);
                            }
                            else
                            {
                                cc.Counts.Add(reader.GetInt32(x));
                            }
                        });

                        chart.Add(cc);
                    }
                }

                _con.Database.CloseConnection();

                return chart;
            }
        }
    }
}
