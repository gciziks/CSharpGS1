using System;
using System.Collections.Generic;

namespace EnergiaMain.Models
{
    public enum ImpactLevel { Low = 1, Medium = 2, High = 3, Critical = 4 }

    public class EnergiaModel
    {
        public int Id { get; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public string Location { get; }
        public string Description { get; }
        public ImpactLevel ImpactLevel { get; private set; }
        public bool IsResolved { get; private set; }
        public string ReportedBy { get; set; }
        public DateTime ReportedAt { get; }
        public List<string> AffectedSystems { get; }

        private static int nextId = 1;

        public EnergiaModel(DateTime startTime, string location, string description,
                               ImpactLevel impactLevel, string reportedBy, List<string> affectedSystems)
        {
            if (startTime > DateTime.Now)
                throw new ArgumentException("A data/hora de início não pode estar no futuro");

            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("O local é obrigatório");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("A descrição é obrigatória");

            if (string.IsNullOrWhiteSpace(reportedBy))
                throw new ArgumentException("É necessário informar quem registrou o evento");

            if (affectedSystems == null || affectedSystems.Count == 0)
                throw new ArgumentException("Pelo menos um sistema afetado deve ser informado");

            Id = nextId++;
            StartTime = startTime;
            Location = location;
            Description = description;
            ImpactLevel = impactLevel;
            ReportedBy = reportedBy;
            ReportedAt = DateTime.Now;
            AffectedSystems = new List<string>(affectedSystems);
        }

        public void MarkAsResolved(DateTime endTime)
        {
            if (endTime < StartTime)
                throw new ArgumentException("A data/hora de término não pode ser anterior à de início");

            EndTime = endTime;
            IsResolved = true;
        }
    }
}
