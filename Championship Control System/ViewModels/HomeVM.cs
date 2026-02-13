using Championship_Control_System.Models;

namespace Championship_Control_System.ViewModels
{
    public class HomeVM
    {
        public List<Match> TodayMatches { get; set; } = new();
        public List<TeamStanding> Standings { get; set; } = new();
    }
}
