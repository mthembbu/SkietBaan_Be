
namespace SkietbaanBE.RequestModel {
    public class AwardObject {
        public string CompetitionName { get; set; }
        public string Total { get; set; }
        public int Accuracy { get; set; }
        public bool IsCompetitionLocked { get; set; }
        public AccuracyAward AccuracyAward { get; set; }
        public TotalAward TotalAward { get; set; }
        public string BestInMonth { get; set; }
    }
}
